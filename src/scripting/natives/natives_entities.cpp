/*
 *  This file is part of CounterStrikeSharp.
 *  CounterStrikeSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  CounterStrikeSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
 */

#include <public/entity2/entitysystem.h>

#include <ios>
#include <sstream>

#include "core/log.h"
#include "core/managers/entity_manager.h"
#include "core/managers/player_manager.h"
#include "core/memory.h"
#include "scripting/autonative.h"
#include "scripting/script_engine.h"

#include "core/recipientfilter.h"
#include "entitykeyvalues.h"

#include "core/game_system.h"

#include "core/global_types.h"

#include "netmessages.pb.h"

namespace counterstrikesharp {

CEntityInstance* GetEntityFromIndex(ScriptContext& script_context)
{
    if (!globals::entitySystem)
    {
        script_context.ThrowNativeError("Entity system is not yet initialized");
        return nullptr;
    }

    auto entityIndex = script_context.GetArgument<int>(0);

    return globals::entitySystem->GetEntityInstance(CEntityIndex(entityIndex));
}

int GetUserIdFromIndex(ScriptContext& scriptContext)
{
    auto entityIndex = scriptContext.GetArgument<int>(0);

    // CPlayerSlot is 1 less than index
    return globals::engine->GetPlayerUserId(CPlayerSlot(entityIndex - 1)).Get();
}

const char* GetDesignerName(ScriptContext& scriptContext)
{
    auto entity = scriptContext.GetArgument<CEntityInstance*>(0);
    return entity->GetClassname();
}

void* GetEntityPointerFromHandle(ScriptContext& scriptContext)
{
    if (!globals::entitySystem)
    {
        scriptContext.ThrowNativeError("Entity system is not yet initialized");
        return nullptr;
    }

    auto handle = scriptContext.GetArgument<CEntityHandle*>(0);

    if (!handle->IsValid())
    {
        return nullptr;
    }

    return globals::entitySystem->GetEntityInstance(*handle);
}

void* GetEntityPointerFromRef(ScriptContext& scriptContext)
{
    if (!globals::entitySystem)
    {
        scriptContext.ThrowNativeError("Entity system yet is not initialized");
        return nullptr;
    }

    auto ref = scriptContext.GetArgument<unsigned int>(0);

    if (ref == INVALID_EHANDLE_INDEX)
    {
        return nullptr;
    }

    CBaseHandle hndl(ref);

    return globals::entitySystem->GetEntityInstance(hndl);
}

unsigned int GetRefFromEntityPointer(ScriptContext& scriptContext)
{
    auto* pEntity = scriptContext.GetArgument<CEntityInstance*>(0);

    if (pEntity == nullptr)
    {
        return INVALID_EHANDLE_INDEX;
    }

    auto hndl = pEntity->GetRefEHandle();

    if (hndl == INVALID_EHANDLE_INDEX)
    {
        return INVALID_EHANDLE_INDEX;
    }

    return hndl.ToInt();
}

bool IsRefValidEntity(ScriptContext& scriptContext)
{
    if (!globals::entitySystem)
    {
        scriptContext.ThrowNativeError("Entity system yet is not initialized");
        return false;
    }

    auto ref = scriptContext.GetArgument<unsigned int>(0);

    if (ref == INVALID_EHANDLE_INDEX)
    {
        return false;
    }

    CBaseHandle hndl(ref);

    if (!hndl.IsValid())
    {
        return false;
    }

    return globals::entitySystem->GetEntityInstance(hndl) != nullptr;
}

void PrintToConsole(ScriptContext& scriptContext)
{
    auto index = scriptContext.GetArgument<int>(0);
    auto message = scriptContext.GetArgument<const char*>(1);

    globals::engine->ClientPrintf(CPlayerSlot{ index - 1 }, message);
}

CEntityIdentity* GetFirstActiveEntity(ScriptContext& script_context)
{
    if (!globals::entitySystem)
    {
        script_context.ThrowNativeError("Entity system yet is not initialized");
        return nullptr;
    }

    return globals::entitySystem->m_EntityList.m_pFirstActiveEntity;
}

void* GetConcreteEntityListPointer(ScriptContext& script_context)
{
    if (!globals::entitySystem)
    {
        script_context.ThrowNativeError("Entity system yet is not initialized");
        return nullptr;
    }

    return &globals::entitySystem->m_EntityList;
}

unsigned long GetPlayerAuthorizedSteamID(ScriptContext& script_context)
{
    auto iSlot = script_context.GetArgument<int>(0);

    auto pPlayer = globals::playerManager.GetPlayerBySlot(iSlot);
    if (pPlayer == nullptr || !pPlayer->m_is_authorized)
    {
        return -1;
    }

    auto pSteamId = pPlayer->GetSteamId();
    if (pSteamId == nullptr)
    {
        return -1;
    }

    return pSteamId->ConvertToUint64();
}

const char* GetPlayerIpAddress(ScriptContext& script_context)
{
    auto iSlot = script_context.GetArgument<int>(0);

    auto pPlayer = globals::playerManager.GetPlayerBySlot(iSlot);
    if (pPlayer == nullptr)
    {
        return nullptr;
    }

    return pPlayer->GetIpAddress();
}

void HookEntityOutput(ScriptContext& script_context)
{
    auto szClassname = script_context.GetArgument<const char*>(0);
    auto szOutput = script_context.GetArgument<const char*>(1);
    auto callback = script_context.GetArgument<CallbackT>(2);
    auto mode = script_context.GetArgument<HookMode>(3);
    globals::entityManager.HookEntityOutput(szClassname, szOutput, callback, mode);
}

void UnhookEntityOutput(ScriptContext& script_context)
{
    auto szClassname = script_context.GetArgument<const char*>(0);
    auto szOutput = script_context.GetArgument<const char*>(1);
    auto callback = script_context.GetArgument<CallbackT>(2);
    auto mode = script_context.GetArgument<HookMode>(3);
    globals::entityManager.UnhookEntityOutput(szClassname, szOutput, callback, mode);
}

void EmitSound(ScriptContext& script_context)
{
    if (!CSoundOpGameSystem_StartSoundEvent)
    {
        script_context.ThrowNativeError("Failed to find signature for \'CSoundOpGameSystem_StartSoundEvent\'");
        return;
    }

    if (!CSoundOpGameSystem_SetSoundEventParam)
    {
        script_context.ThrowNativeError("Failed to find signature for \'CSoundOpGameSystem_SetSoundEventParam\'");
        return;
    }

    IGameSystem* pSoundOpGameSystem = CBaseGameSystemFactory::GetGlobalPtrByName("SoundOpGameSystem");
    if (!pSoundOpGameSystem)
    {
        script_context.ThrowNativeError("Failed to locate \'SoundOpGameSystem\'");
        return;
    }

    auto entIndex = script_context.GetArgument<unsigned int>(0);
    auto soundName = script_context.GetArgument<const char*>(1);
    auto pitch = script_context.GetArgument<float>(2);
    auto volume = script_context.GetArgument<float>(3);
    auto suppliedCustomFilter = script_context.GetArgument<bool>(4);

    CRecipientFilter filter;
    
    // If managed side defined recipient players, add them
    if (suppliedCustomFilter)
    {
        auto recipients = script_context.GetArgument<uint64>(5);
        filter.AddRecipientsByMasking(recipients);
    } else // else we add all the valid players into filter
        filter.AddAllPlayers();

    SndOpEventGuid_t guid;
    CSoundOpGameSystem_StartSoundEvent(pSoundOpGameSystem, &guid, &filter, soundName, entIndex, -1, 0);

    SoundEventParamFloat _pitch = SoundEventParamFloat(pitch);
    bool result = CSoundOpGameSystem_SetSoundEventParam(pSoundOpGameSystem, &filter, guid, "pitch", &_pitch, 0, 0);
    if (!result)
        CSSHARP_CORE_ERROR("Failed to SetSoundEventParam ({}, {}, {:.2f}) | GUID {} | HASH {:#x}", soundName, "pitch", pitch, guid.m_nGuid,
                           guid.m_hStackHash);

    SoundEventParamFloat _volume = SoundEventParamFloat(volume);
    result = CSoundOpGameSystem_SetSoundEventParam(pSoundOpGameSystem, &filter, guid, "volume", &_volume, 0, 0);
    if (!result)
        CSSHARP_CORE_ERROR("Failed to SetSoundEventParam ({}, {}, {:.2f}) | GUID {} | HASH {:#x}", soundName, "volume", volume,
                           guid.m_nGuid, guid.m_hStackHash);
}

#pragma pack(push)
#pragma pack(1)
struct EKVEntry {
    const char* key;
    KeyValuesType_t type;
    union
    {
        bool boolValue;
        int32 intValue;
        uint uintValue; // Shared with EHandle, StringToken
        int64 int64Value;
        uint64 uint64Value;
        float floatValue;
        double doubleValue;
        const char* stringValue;
        void* pointerValue;
        Color colorValue;
        Vector2D vec2Value;
        Vector vec3Value;
        Vector4D vec4Value;
        Quaternion quaternionValue;
        QAngle angleValue;
        matrix3x4_t matrix3x4Value;
    } value;
};
#pragma pack(pop)

void DispatchSpawn(ScriptContext& script_context)
{
    if (!CBaseEntity_DispatchSpawn)
    {
        script_context.ThrowNativeError("Failed to find signature for \'CBaseEntity_DispatchSpawn\'");
        return;
    }

    auto pEntity = script_context.GetArgument<CEntityInstance*>(0);
    auto count = script_context.GetArgument<int>(1);

    if (count == 0) {
        CBaseEntity_DispatchSpawn(pEntity, nullptr);
        return;
    }

    CEntityKeyValues* pEntityKeyValues = new CEntityKeyValues();

    auto entryPtrs = script_context.GetArgument<EKVEntry**>(2);
    for (int i = 0; i < count; ++i)
    {
        EKVEntry* entry = entryPtrs[i];
        const char* key = entry->key;
        switch (entry->type)
        {
            case TYPE_BOOL:
                pEntityKeyValues->SetBool(key, entry->value.boolValue);
                break;

            case TYPE_INT:
                pEntityKeyValues->SetInt(key, entry->value.intValue);
                break;

            case TYPE_UINT:
                pEntityKeyValues->SetUint(key, entry->value.uintValue);
                break;

            case TYPE_INT64:
                pEntityKeyValues->SetInt64(key, entry->value.int64Value);
                break;

            case TYPE_UINT64:
                pEntityKeyValues->SetUint64(key, entry->value.uint64Value);
                break;

            case TYPE_FLOAT:
                pEntityKeyValues->SetFloat(key, entry->value.floatValue);
                break;

            case TYPE_DOUBLE:
                pEntityKeyValues->SetDouble(key, entry->value.doubleValue);
                break;

            case TYPE_STRING:
                pEntityKeyValues->SetString(key, entry->value.stringValue);
                break;

            case TYPE_POINTER:
                pEntityKeyValues->SetPtr(key, entry->value.pointerValue);
                break;

            case TYPE_STRING_TOKEN:
                pEntityKeyValues->SetStringToken(key, entry->value.uintValue);
                break;

            case TYPE_EHANDLE:
                pEntityKeyValues->SetEHandle(key, entry->value.uintValue);
                break;

            case TYPE_COLOR:
                pEntityKeyValues->SetColor(key, entry->value.colorValue);
                break;

            case TYPE_VECTOR:
                pEntityKeyValues->SetVector(key, entry->value.vec3Value);
                break;

            case TYPE_VECTOR2D:
                pEntityKeyValues->SetVector2D(key, entry->value.vec2Value);
                break;

            case TYPE_VECTOR4D:
                pEntityKeyValues->SetVector4D(key, entry->value.vec4Value);
                break;

            case TYPE_QUATERNION:
                pEntityKeyValues->SetQuaternion(key, entry->value.quaternionValue);
                break;

            case TYPE_QANGLE:
                pEntityKeyValues->SetQAngle(key, entry->value.angleValue);
                break;

            case TYPE_MATRIX3X4:
                pEntityKeyValues->SetMatrix3x4(key, entry->value.matrix3x4Value);
                break;

            default:
                break;
        }
    }

    CBaseEntity_DispatchSpawn(pEntity, pEntityKeyValues);
}

void* CreateWeaponEntity(ScriptContext& script_context)
{
    if (!CItemSelectionCriteria_BAddCondition)
    {
        CSSHARP_CORE_CRITICAL("Failed to find signature for \'CItemSelectionCriteria_BAddCondition\'");
        return nullptr;
    }

    if (!CItemGeneration_GenerateRandomItem)
    {
        CSSHARP_CORE_CRITICAL("Failed to find signature for \'CItemGeneration_GenerateRandomItem\'");
        return nullptr;
    }

    void* itemGenerationPtr = CBaseGameSystemFactory::GetGlobalPtrByName("CItemGeneration");
    if (!itemGenerationPtr)
    {
        CSSHARP_CORE_CRITICAL("Invalid 'CItemGeneration' pointer");
        return nullptr;
    }
    
    CItemSelectionCriteria criteria;
    criteria.m_nItemQuality = AE_NORMAL;
    criteria.m_bQualitySet = true;
    CItemSelectionCriteria_BAddCondition(&criteria, "name", k_EOperator_String_EQ, script_context.GetArgument<const char*>(0), true);

    void* entity = CItemGeneration_GenerateRandomItem(itemGenerationPtr, &criteria, vec3_origin, vec3_angle);

    if (!entity)
    {
        criteria.m_nItemQuality = AE_UNIQUE;
        entity = CItemGeneration_GenerateRandomItem(itemGenerationPtr, &criteria, vec3_origin, vec3_angle);
    }

    return entity;
}

int DispatchParticle(ScriptContext& script_context)
{
    if (!DispatchParticleEffect)
    {
        script_context.ThrowNativeError("Failed to find signature for \'DispatchParticleEffect\'");
        return 0;
    }

    CEntityInstance* entity = script_context.GetArgument<CEntityInstance*>(0);
    const char* pszParticleName = script_context.GetArgument<const char*>(1);
    uint64 filter = script_context.GetArgument<uint64>(2);
    ParticleAttachment_t nAttachType = script_context.GetArgument<ParticleAttachment_t>(3);
    uint8 iAttachmentPoint = script_context.GetArgument<uint8>(4);
    const char* pszAttachmentName = script_context.GetArgument<const char*>(5);

    CRecipientFilter _filter;
    _filter.AddRecipientsByMasking(filter);
    CUtlSymbolLarge attachmentName(pszAttachmentName);

    return DispatchParticleEffect(pszParticleName, nAttachType, entity, iAttachmentPoint, attachmentName, false, 0, &_filter, 0);
}

int DispatchParticle2(ScriptContext& script_context)
{
    if (!DispatchParticleEffect2)
    {
        script_context.ThrowNativeError("Failed to find signature for \'DispatchParticleEffect2\'");
        return 0;
    }

    CEntityInstance* entity = script_context.GetArgument<CEntityInstance*>(0);
    const char* pszParticleName = script_context.GetArgument<const char*>(1);
    uint64 filter = script_context.GetArgument<uint64>(2);
    Vector* vec = script_context.GetArgument<Vector*>(3);
    QAngle* ang = script_context.GetArgument<QAngle*>(4);

    CRecipientFilter _filter;
    _filter.AddRecipientsByMasking(filter);

    return DispatchParticleEffect2(pszParticleName, vec, ang, entity, false, -1, &_filter, 0);
}

void QueryConvarValue(ScriptContext& script_context)
{
    int slot = script_context.GetArgument<int>(0);
    if (slot < 0 || slot > 63)
    {
        script_context.ThrowNativeError("Invalid player slot {}", slot);
        return;
    }

    const char* cvar = script_context.GetArgument<const char*>(1);
    auto callback = script_context.GetArgument<void*>(2);

    CServerSideClient* client = globals::GetClientBySlot(slot);

    if (!client)
    {
        script_context.ThrowNativeError("Couldn't get player from slot {}", slot);
        return;
    }

    static INetworkMessageInternal* pMsg = globals::networkMessages->FindNetworkMessagePartial("CSVCMsg_GetCvarValue");

    static int iQueryCvarCookieCounter = 0;
    int iQueryCvarCookie = ++iQueryCvarCookieCounter;

    // Add callback
    globals::entityManager.m_CvarQueryLists[iQueryCvarCookie] = callback;

    CNetMessagePB<CSVCMsg_GetCvarValue>* msg = pMsg->AllocateMessage()->ToPB<CSVCMsg_GetCvarValue>();
    msg->set_cookie(iQueryCvarCookie);
    msg->set_cvar_name(cvar);

    client->GetNetChannel()->SendNetMessage(msg, BUF_RELIABLE);

    delete msg;
}

void AcceptInput(ScriptContext& script_context)
{
    if (!CEntityInstance_AcceptInput)
    {
        script_context.ThrowNativeError("Failed to find signature for \'CEntityInstance_AcceptInput\'");
        return;
    }

    CEntityInstance* pThis = script_context.GetArgument<CEntityInstance*>(0);
    const char* pInputName = script_context.GetArgument<const char*>(1);
    CEntityInstance* pActivator = script_context.GetArgument<CEntityInstance*>(2);
    CEntityInstance* pCaller = script_context.GetArgument<CEntityInstance*>(3);
    const char* value = script_context.GetArgument<const char*>(4);
    int outputID = script_context.GetArgument<int>(5);

    variant_t _value = variant_t(value);
    CEntityInstance_AcceptInput(pThis, pInputName, pActivator, pCaller, &_value, outputID);
}

void AddEntityIOEvent(ScriptContext& script_context)
{
    if (!CEntitySystem_AddEntityIOEvent)
    {
        script_context.ThrowNativeError("Failed to find signature for \'CEntitySystem_AddEntityIOEvent\'");
        return;
    }

    CEntityInstance* pTarget = script_context.GetArgument<CEntityInstance*>(0);
    const char* pInputName = script_context.GetArgument<const char*>(1);
    CEntityInstance* pActivator = script_context.GetArgument<CEntityInstance*>(2);
    CEntityInstance* pCaller = script_context.GetArgument<CEntityInstance*>(3);
    const char* value = script_context.GetArgument<const char*>(4);
    float delay = script_context.GetArgument<float>(5);
    int outputID = script_context.GetArgument<int>(6);

    variant_t _value = variant_t(value);
    CEntitySystem_AddEntityIOEvent(GameEntitySystem(), pTarget, pInputName, pActivator, pCaller, &_value, delay, outputID);
}

REGISTER_NATIVES(entities, {
    ScriptEngine::RegisterNativeHandler("GET_ENTITY_FROM_INDEX", GetEntityFromIndex);
    ScriptEngine::RegisterNativeHandler("GET_USERID_FROM_INDEX", GetUserIdFromIndex);
    ScriptEngine::RegisterNativeHandler("GET_DESIGNER_NAME", GetDesignerName);
    ScriptEngine::RegisterNativeHandler("GET_ENTITY_POINTER_FROM_HANDLE", GetEntityPointerFromHandle);
    ScriptEngine::RegisterNativeHandler("GET_ENTITY_POINTER_FROM_REF", GetEntityPointerFromRef);
    ScriptEngine::RegisterNativeHandler("GET_REF_FROM_ENTITY_POINTER", GetRefFromEntityPointer);
    ScriptEngine::RegisterNativeHandler("GET_CONCRETE_ENTITY_LIST_POINTER", GetConcreteEntityListPointer);
    ScriptEngine::RegisterNativeHandler("IS_REF_VALID_ENTITY", IsRefValidEntity);
    ScriptEngine::RegisterNativeHandler("PRINT_TO_CONSOLE", PrintToConsole);
    ScriptEngine::RegisterNativeHandler("GET_FIRST_ACTIVE_ENTITY", GetFirstActiveEntity);
    ScriptEngine::RegisterNativeHandler("GET_PLAYER_AUTHORIZED_STEAMID", GetPlayerAuthorizedSteamID);
    ScriptEngine::RegisterNativeHandler("GET_PLAYER_IP_ADDRESS", GetPlayerIpAddress);
    ScriptEngine::RegisterNativeHandler("HOOK_ENTITY_OUTPUT", HookEntityOutput);
    ScriptEngine::RegisterNativeHandler("UNHOOK_ENTITY_OUTPUT", UnhookEntityOutput);
    ScriptEngine::RegisterNativeHandler("EMIT_SOUND", EmitSound);
    ScriptEngine::RegisterNativeHandler("DISPATCH_SPAWN", DispatchSpawn);
    ScriptEngine::RegisterNativeHandler("CREATE_WEAPON_ENTITY", CreateWeaponEntity);
    ScriptEngine::RegisterNativeHandler("ACCEPT_INPUT", AcceptInput);
    ScriptEngine::RegisterNativeHandler("ADD_ENTITY_IO_EVENT", AddEntityIOEvent);
    ScriptEngine::RegisterNativeHandler("QUERY_CONVAR_VALUE", QueryConvarValue);
    ScriptEngine::RegisterNativeHandler("DISPATCH_PARTICLE", DispatchParticle);
    ScriptEngine::RegisterNativeHandler("DISPATCH_PARTICLE2", DispatchParticle2);
})
} // namespace counterstrikesharp
