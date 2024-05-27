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
        auto recipientCount = script_context.GetArgument<int>(5);

        for (int i = 0; i < recipientCount; ++i)
            filter.AddRecipient(script_context.GetArgument<int>(6 + i));
    } else // else we add all the valid players into filter
    {
        filter.AddAllPlayers();
    }

    SndOpEventGuid_t guid;
    CSoundOpGameSystem_StartSoundEvent(pSoundOpGameSystem, &guid, &filter, soundName, entIndex, -1, 0);

    SoundEventParamFloat _pitch = SoundEventParamFloat(pitch);
    bool result = CSoundOpGameSystem_SetSoundEventParam(pSoundOpGameSystem, &filter, guid, "pitch", &_pitch, 0, 0);
    if (!result)
    {
        CSSHARP_CORE_ERROR("Failed to SetSoundEventParam (%s, %s, %.2f) | GUID %d | HASH %x", soundName, "pitch", pitch, guid.m_nGuid, guid.m_hStackHash);
    }

    SoundEventParamFloat _volume = SoundEventParamFloat(volume);
    result = CSoundOpGameSystem_SetSoundEventParam(pSoundOpGameSystem, &filter, guid, "volume", &_volume, 0, 0);
    if (!result)
    {
        CSSHARP_CORE_ERROR("Failed to SetSoundEventParam (%s, %s, %.2f) | GUID %d | HASH %x", soundName, "volume", volume, guid.m_nGuid, guid.m_hStackHash);
    }
}

enum KeyValuesType_t : unsigned int
{
    TYPE_BOOL,
    TYPE_INT,
    TYPE_UINT,
    TYPE_INT64,
    TYPE_UINT64,
    TYPE_FLOAT,
    TYPE_DOUBLE,
    TYPE_STRING,
    TYPE_POINTER,
    TYPE_STRING_TOKEN,
    TYPE_EHANDLE,
    TYPE_COLOR,
    TYPE_VECTOR,
    TYPE_VECTOR2D,
    TYPE_VECTOR4D,
    TYPE_QUATERNION,
    TYPE_QANGLE,
    TYPE_MATRIX3X4
};

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
    int offset = 2;
    for (int i = 0; i < count; ++i) {
        const char* key = script_context.GetArgument<const char*>(offset);
        KeyValuesType_t _type = script_context.GetArgument<KeyValuesType_t>(offset + 1);
        switch (_type) {
        case counterstrikesharp::TYPE_BOOL:
            pEntityKeyValues->SetBool(key, script_context.GetArgument<bool>(offset + 2));
            offset += 3;
            break;
        case counterstrikesharp::TYPE_INT:
            pEntityKeyValues->SetInt(key, script_context.GetArgument<int>(offset + 2));
            offset += 3;
            break;
        case counterstrikesharp::TYPE_UINT:
            pEntityKeyValues->SetUint(key, script_context.GetArgument<unsigned int>(offset + 2));
            offset += 3;
            break;
        case counterstrikesharp::TYPE_INT64:
            pEntityKeyValues->SetInt64(key, script_context.GetArgument<long>(offset + 2));
            offset += 3;
            break;
        case counterstrikesharp::TYPE_UINT64:
            pEntityKeyValues->SetUint64(key, script_context.GetArgument<unsigned long>(offset + 2));
            offset += 3;
            break;
        case counterstrikesharp::TYPE_FLOAT:
            pEntityKeyValues->SetFloat(key, script_context.GetArgument<float>(offset + 2));
            offset += 3;
            break;
        case counterstrikesharp::TYPE_DOUBLE:
            pEntityKeyValues->SetDouble(key, script_context.GetArgument<double>(offset + 2));
            offset += 3;
            break;
        case counterstrikesharp::TYPE_STRING:
            pEntityKeyValues->SetString(key, ((CUtlString*)new CUtlString(script_context.GetArgument<const char*>(offset + 2)))->Get());
            offset += 3;
            break;
        case counterstrikesharp::TYPE_POINTER:
            pEntityKeyValues->SetPtr(key, script_context.GetArgument<void*>(offset + 2));
            offset += 3;
            break;
        case counterstrikesharp::TYPE_STRING_TOKEN:
            pEntityKeyValues->SetStringToken(key, script_context.GetArgument<unsigned int>(offset + 2));
            offset += 3;
            break;
        case counterstrikesharp::TYPE_EHANDLE:
            pEntityKeyValues->SetEHandle(key, script_context.GetArgument<unsigned int>(offset + 2));
            offset += 3;
            break;
        case counterstrikesharp::TYPE_COLOR: {
            char r = script_context.GetArgument<char>(offset + 2);
            char g = script_context.GetArgument<char>(offset + 3);
            char b = script_context.GetArgument<char>(offset + 4);
            char a = script_context.GetArgument<char>(offset + 5);

            pEntityKeyValues->SetColor(key, Color(r, g, b, a));
            offset += 6;
            break;
        }
        case counterstrikesharp::TYPE_VECTOR: {
            float x = script_context.GetArgument<float>(offset + 2);
            float y = script_context.GetArgument<float>(offset + 3);
            float z = script_context.GetArgument<float>(offset + 4);

            pEntityKeyValues->SetVector(key, Vector(x, y, z));
            offset += 5;
            break;
        }
        case counterstrikesharp::TYPE_VECTOR2D: {
            float x = script_context.GetArgument<float>(offset + 2);
            float y = script_context.GetArgument<float>(offset + 3);

            pEntityKeyValues->SetVector2D(key, Vector2D(x, y));
            offset += 4;
            break;
        }
        case counterstrikesharp::TYPE_VECTOR4D: {
            float x = script_context.GetArgument<float>(offset + 2);
            float y = script_context.GetArgument<float>(offset + 3);
            float z = script_context.GetArgument<float>(offset + 4);
            float w = script_context.GetArgument<float>(offset + 5);

            pEntityKeyValues->SetVector4D(key, Vector4D(x, y, z, w));
            offset += 6;
            break;
        }
        case counterstrikesharp::TYPE_QUATERNION: {
            float x = script_context.GetArgument<float>(offset + 2);
            float y = script_context.GetArgument<float>(offset + 3);
            float z = script_context.GetArgument<float>(offset + 4);
            float w = script_context.GetArgument<float>(offset + 5);

            pEntityKeyValues->SetQuaternion(key, Quaternion(x, y, z, w));
            offset += 6;
            break;
        }
        case counterstrikesharp::TYPE_QANGLE: {
            float x = script_context.GetArgument<float>(offset + 2);
            float y = script_context.GetArgument<float>(offset + 3);
            float z = script_context.GetArgument<float>(offset + 4);

            pEntityKeyValues->SetQAngle(key, QAngle(x, y, z));
            offset += 5;
            break;
        }
        case counterstrikesharp::TYPE_MATRIX3X4: {
            float m11 = script_context.GetArgument<float>(offset + 2);
            float m12 = script_context.GetArgument<float>(offset + 3);
            float m13 = script_context.GetArgument<float>(offset + 4);
            float m14 = script_context.GetArgument<float>(offset + 5);

            float m21 = script_context.GetArgument<float>(offset + 6);
            float m22 = script_context.GetArgument<float>(offset + 7);
            float m23 = script_context.GetArgument<float>(offset + 8);
            float m24 = script_context.GetArgument<float>(offset + 9);

            float m31 = script_context.GetArgument<float>(offset + 10);
            float m32 = script_context.GetArgument<float>(offset + 11);
            float m33 = script_context.GetArgument<float>(offset + 12);
            float m34 = script_context.GetArgument<float>(offset + 13);

            pEntityKeyValues->SetMatrix3x4(
                key, matrix3x4_t(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34));
            offset += 14;
            break;
        }
        default:
            break;
        }
    }

    CBaseEntity_DispatchSpawn(pEntity, pEntityKeyValues);
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
    ScriptEngine::RegisterNativeHandler("ACCEPT_INPUT", AcceptInput);
    ScriptEngine::RegisterNativeHandler("ADD_ENTITY_IO_EVENT", AddEntityIOEvent);
})
} // namespace counterstrikesharp
