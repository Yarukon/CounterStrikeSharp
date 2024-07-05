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

#include <IEngineSound.h>
#include <edict.h>
#include <eiface.h>
#include <filesystem.h>
#include <public/worldsize.h>

#include "entitykeyvalues.h"

// clang-format off
#include "mm_plugin.h"
#include "core/timer_system.h"
#include "core/utils.h"
#include "scripting/autonative.h"
#include "scripting/script_engine.h"
#include "core/memory.h"
#include "core/log.h"
#include "core/function.h"
#include "core/managers/player_manager.h"
#include "core/managers/server_manager.h"
#include "core/tick_scheduler.h"
// clang-format on

#include "core/serversideclient.h"
#include "networkbasetypes.pb.h"

#if _WIN32
#undef GetCurrentTime
#endif

namespace counterstrikesharp {

const char* GetMapName(ScriptContext& script_context)
{
    if (globals::GetGlobalVars() == nullptr) return nullptr;

    return globals::GetGlobalVars()->mapname.ToCStr();
}

const char* GetGameDirectory(ScriptContext& script_context) { return strdup(Plat_GetGameDirectory()); }

bool IsMapValid(ScriptContext& script_context)
{
    auto mapname = script_context.GetArgument<const char*>(0);
    return globals::engine->IsMapValid(mapname) != 0;
}

float GetTickInterval(ScriptContext& script_context) { return globals::engine_fixed_tick_interval; }

float GetCurrentTime(ScriptContext& script_context) { return globals::GetGlobalVars()->curtime; }

int GetTickCount(ScriptContext& script_context) { return globals::GetGlobalVars()->tickcount; }

float GetGameFrameTime(ScriptContext& script_context) { return globals::GetGlobalVars()->frametime; }

double GetEngineTime(ScriptContext& script_context) { return Plat_FloatTime(); }

int GetMaxClients(ScriptContext& script_context)
{
    auto globalVars = globals::GetGlobalVars();
    if (globalVars == nullptr)
    {
        script_context.ThrowNativeError("Global Variables not initialized yet.");
        return -1;
    }

    return globalVars->maxClients;
}

void ServerCommand(ScriptContext& script_context)
{
    auto command = script_context.GetArgument<const char*>(0);

    auto clean_command = std::string(command);
    clean_command.append("\n\0");
    globals::engine->ServerCommand(clean_command.c_str());
}

void PrecacheModel(ScriptContext& script_context)
{
    auto name = script_context.GetArgument<const char*>(0);
    globals::engine->PrecacheGeneric(name);
}

bool PrecacheSound(ScriptContext& script_context)
{
    auto [name, preload] = script_context.GetArguments<const char*, bool>();

    return globals::engineSound->PrecacheSound(name, preload);
}

bool IsSoundPrecached(ScriptContext& script_context)
{
    auto name = script_context.GetArgument<const char*>(0);

    return globals::engineSound->IsSoundPrecached(name);
}

float GetSoundDuration(ScriptContext& script_context)
{
    auto name = script_context.GetArgument<const char*>(0);

    return globals::engineSound->GetSoundDuration(name);
}

double GetTickedTime(ScriptContext& script_context) { return globals::timerSystem.GetTickedTime(); }

void QueueTaskForNextFrame(ScriptContext& script_context)
{
    auto func = script_context.GetArgument<void*>(0);

    typedef void(voidfunc)(void);
    globals::mmPlugin->AddTaskForNextFrame([func]() {
        reinterpret_cast<voidfunc*>(func)();
    });
}

void QueueTaskForNextWorldUpdate(ScriptContext& script_context)
{
    auto func = script_context.GetArgument<void*>(0);

    typedef void(voidfunc)(void);
    globals::serverManager.AddTaskForNextWorldUpdate([func]() {
        reinterpret_cast<voidfunc*>(func)();
    });
}

void QueueTaskForFrame(ScriptContext& script_context)
{
    auto tick = script_context.GetArgument<int>(0);
    auto func = script_context.GetArgument<void*>(1);

    typedef void(voidfunc)(void);
    globals::tickScheduler.schedule(tick, reinterpret_cast<voidfunc*>(func));
}

enum InterfaceType
{
    Engine,
    Server
};

void* GetValveInterface(ScriptContext& scriptContext)
{
    auto [interfaceType, interfaceName] = scriptContext.GetArguments<InterfaceType, const char*>();

    CreateInterfaceFn factoryFn;
    if (interfaceType == Server)
    {
        factoryFn = globals::ismm->GetServerFactory();
    }
    else if (interfaceType == Engine)
    {
        factoryFn = globals::ismm->GetEngineFactory();
    }

    auto foundInterface = globals::ismm->VInterfaceMatch(factoryFn, interfaceName);

    if (foundInterface == nullptr)
    {
        scriptContext.ThrowNativeError("Could not find interface");
    }

    return foundInterface;
}

void GetCommandParamValue(ScriptContext& scriptContext)
{
    auto paramName = scriptContext.GetArgument<const char*>(0);
    auto paramType = scriptContext.GetArgument<DataType_t>(1);

    int iContextIndex = 2;
    switch (paramType)
    {
        case DATA_TYPE_STRING:
            scriptContext.SetResult(CommandLine()->ParmValue(paramName, scriptContext.GetArgument<const char*>(iContextIndex)));
            return;
        case DATA_TYPE_INT:
            scriptContext.SetResult(CommandLine()->ParmValue(paramName, scriptContext.GetArgument<int>(iContextIndex)));
            return;
        case DATA_TYPE_FLOAT:
            scriptContext.SetResult(CommandLine()->ParmValue(paramName, scriptContext.GetArgument<float>(iContextIndex)));
            return;
    }

    scriptContext.ThrowNativeError("Invalid param type");
}

void PrintToServerConsole(ScriptContext& scriptContext)
{
    auto message = scriptContext.GetArgument<const char*>(0);

    META_CONPRINT(message);
}

void* GetFirstGameSystemPtr(ScriptContext& scriptContext)
{
    if (!*CBaseGameSystemFactory::sm_pFirst) return nullptr;
    return *CBaseGameSystemFactory::sm_pFirst;
}

void ReplicateToClient(ScriptContext& scriptContext)
{
    int slot = scriptContext.GetArgument<int>(0);
    if (slot < 0 || slot > 63)
    {
        scriptContext.ThrowNativeError("Player slot %d out of range", slot);
        return;
    }

    CPlayer* player = globals::playerManager.GetPlayerBySlot(slot);

    if (!player || !player->IsConnected() || player->IsFakeClient())
    {
        scriptContext.ThrowNativeError("Invalid player for slot %d", slot);
        return;
    }

    CServerSideClient* pClient = globals::GetClientBySlot(player->m_slot);
    if (!pClient)
    {
        scriptContext.ThrowNativeError("Couldn't find client %s", player->GetName());
        return;
    }

    INetworkMessageInternal* netMsg = globals::networkMessages->FindNetworkMessagePartial("SetConVar");
    if (!netMsg)
    {
        scriptContext.ThrowNativeError("Couldn't find network message for this to work!");
        return;
    }

    auto msg = netMsg->AllocateMessage()->ToPB<CNETMsg_SetConVar>();
    CMsg_CVars_CVar* cvar = msg->mutable_convars()->add_cvars();
    cvar->set_name(scriptContext.GetArgument<const char*>(1));
    cvar->set_value(scriptContext.GetArgument<const char*>(2));

    pClient->GetNetChannel()->SendNetMessage(netMsg, msg, BUF_RELIABLE);
}

REGISTER_NATIVES(engine, {
    ScriptEngine::RegisterNativeHandler("GET_GAME_DIRECTORY", GetGameDirectory);
    ScriptEngine::RegisterNativeHandler("GET_MAP_NAME", GetMapName);
    ScriptEngine::RegisterNativeHandler("IS_MAP_VALID", IsMapValid);
    ScriptEngine::RegisterNativeHandler("GET_TICK_INTERVAL", GetTickInterval);
    ScriptEngine::RegisterNativeHandler("GET_TICK_COUNT", GetTickCount);
    ScriptEngine::RegisterNativeHandler("GET_CURRENT_TIME", GetCurrentTime);
    ScriptEngine::RegisterNativeHandler("GET_GAMEFRAME_TIME", GetGameFrameTime);
    ScriptEngine::RegisterNativeHandler("GET_ENGINE_TIME", GetEngineTime);
    ScriptEngine::RegisterNativeHandler("GET_MAX_CLIENTS", GetMaxClients);
    ScriptEngine::RegisterNativeHandler("ISSUE_SERVER_COMMAND", ServerCommand);
    ScriptEngine::RegisterNativeHandler("PRECACHE_MODEL", PrecacheModel);
    ScriptEngine::RegisterNativeHandler("PRECACHE_SOUND", PrecacheSound);
    ScriptEngine::RegisterNativeHandler("IS_SOUND_PRECACHED", IsSoundPrecached);
    ScriptEngine::RegisterNativeHandler("GET_SOUND_DURATION", GetSoundDuration);

    ScriptEngine::RegisterNativeHandler("GET_TICKED_TIME", GetTickedTime);
    ScriptEngine::RegisterNativeHandler("QUEUE_TASK_FOR_NEXT_FRAME", QueueTaskForNextFrame);
    ScriptEngine::RegisterNativeHandler("QUEUE_TASK_FOR_NEXT_WORLD_UPDATE", QueueTaskForNextWorldUpdate);
    ScriptEngine::RegisterNativeHandler("QUEUE_TASK_FOR_FRAME", QueueTaskForFrame);
    ScriptEngine::RegisterNativeHandler("GET_VALVE_INTERFACE", GetValveInterface);
    ScriptEngine::RegisterNativeHandler("GET_COMMAND_PARAM_VALUE", GetCommandParamValue);
    ScriptEngine::RegisterNativeHandler("PRINT_TO_SERVER_CONSOLE", PrintToServerConsole);
    ScriptEngine::RegisterNativeHandler("GET_FIRST_GAMESYSTEM_PTR", GetFirstGameSystemPtr);
    ScriptEngine::RegisterNativeHandler("REPLICATE_TO_CLIENT", ReplicateToClient);
})
} // namespace counterstrikesharp
