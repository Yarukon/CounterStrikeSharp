/*
 *  This file is part of VSP.NET.
 *  VSP.NET is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  VSP.NET is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with VSP.NET.  If not, see <https://www.gnu.org/licenses/>. *
 */

#include <eiface.h>

#include "scripting/autonative.h"
#include "scripting/callback_manager.h"
#include "core/managers/con_command_manager.h"
#include "core/managers/player_manager.h"
#include "scripting/script_engine.h"
#include "core/log.h"

#include "tier0/memdbgon.h"

namespace counterstrikesharp {

static void AddCommand(ScriptContext& script_context)
{
    auto name = script_context.GetArgument<const char*>(0);
    auto description = script_context.GetArgument<const char*>(1);
    auto server_only = script_context.GetArgument<bool>(2);
    auto flags = script_context.GetArgument<int>(3);
    auto callback = script_context.GetArgument<CallbackT>(4);

    CSSHARP_CORE_TRACE("Adding command {}, {}, {}, {}, {}", name, description, server_only, flags,
                       (void*)callback);

    globals::conCommandManager.AddValveCommand(name, description, server_only, flags);
    globals::conCommandManager.AddCommandListener(name, callback, HookMode::Pre);
}

static void RemoveCommand(ScriptContext& script_context)
{
    auto name = script_context.GetArgument<const char*>(0);
    auto callback = script_context.GetArgument<CallbackT>(1);

    globals::conCommandManager.RemoveCommandListener(name, callback, HookMode::Pre);
    globals::conCommandManager.RemoveValveCommand(name);
}

static void AddCommandListener(ScriptContext& script_context)
{
    auto name = script_context.GetArgument<const char*>(0);
    auto callback = script_context.GetArgument<CallbackT>(1);
    auto post = script_context.GetArgument<bool>(2);

    globals::conCommandManager.AddCommandListener(name, callback,
                                                  post ? HookMode::Post : HookMode::Pre);
}

static void RemoveCommandListener(ScriptContext& script_context)
{
    auto name = script_context.GetArgument<const char*>(0);
    auto callback = script_context.GetArgument<CallbackT>(1);
    auto post = script_context.GetArgument<bool>(2);

    globals::conCommandManager.RemoveCommandListener(name, callback,
                                                     post ? HookMode::Post : HookMode::Pre);
}

static int CommandGetArgCount(ScriptContext& script_context)
{
    auto command = script_context.GetArgument<CCommand*>(0);

    if (!command) {
        script_context.ThrowNativeError("Invalid command.");
        return -1;
    }

    return command->ArgC();
}

static const char* CommandGetArgString(ScriptContext& script_context)
{
    auto command = script_context.GetArgument<CCommand*>(0);

    if (!command) {
        script_context.ThrowNativeError("Invalid command.");
        return nullptr;
    }

    return command->ArgS();
}

static const char* CommandGetCommandString(ScriptContext& script_context)
{
    auto* command = script_context.GetArgument<CCommand*>(0);

    if (!command) {
        script_context.ThrowNativeError("Invalid command.");
        return nullptr;
    }

    return command->GetCommandString();
}

static const char* CommandGetArgByIndex(ScriptContext& script_context)
{
    auto* command = script_context.GetArgument<CCommand*>(0);
    auto index = script_context.GetArgument<int>(1);

    if (!command) {
        script_context.ThrowNativeError("Invalid command.");
        return nullptr;
    }

    return command->Arg(index);
}

static CommandCallingContext CommandGetCallingContext(ScriptContext& script_context)
{
    auto* command = script_context.GetArgument<CCommand*>(0);

    return globals::conCommandManager.GetCommandCallingContext(command);
}

static void IssueClientCommand(ScriptContext& script_context)
{
    auto slot = script_context.GetArgument<int>(0);
    auto command = script_context.GetArgument<const char*>(1);

    globals::engine->ClientCommand(CPlayerSlot(slot), command);
}

static void IssueClientCommandFromServer(ScriptContext& script_context)
{
    auto slot = script_context.GetArgument<int>(0);
    auto pszCommand = script_context.GetArgument<const char*>(1);

    CCommand args;
    args.Tokenize(pszCommand);

    auto handle = globals::cvars->FindCommand(args.Arg(0));
    if (!handle.IsValid())
        return;

    CCommandContext context(CommandTarget_t::CT_NO_TARGET, CPlayerSlot(slot));

    globals::cvars->DispatchConCommand(handle, context, args);
}

ConVar* FindConVar(ScriptContext& script_context)
{
    auto name = script_context.GetArgument<const char*>(0);
    auto hCvarHandle = globals::cvars->FindConVar(name, true);

    if (!hCvarHandle.IsValid()) {
        return nullptr;
    }

    return globals::cvars->GetConVar(hCvarHandle);
}

void SetConVarStringValue(ScriptContext& script_context)
{
    auto pCvar = script_context.GetArgument<ConVar*>(0);
    auto value = script_context.GetArgument<const char*>(1);

    if (!pCvar) {
        script_context.ThrowNativeError("Invalid cvar.");
    }

    pCvar->values = reinterpret_cast<CVValue_t**>((char*)value);
}

ConVar* GetConVarFromHandle(ScriptContext& script_context)
{
    ConVarHandle hCvarHandle = script_context.GetArgument<ConVarHandle>(0);
    if (!hCvarHandle.IsValid())
    {
        return nullptr;
    }

    return globals::cvars->GetConVar(hCvarHandle);
}

uint32 GetFirstConVar(ScriptContext& script_context)
{
    ConVarHandle hCvarHandle = globals::cvars->FindFirstConVar();

    if (hCvarHandle.IsValid())
    {
        return hCvarHandle.Get();
    }

    return -1;
}

uint32 GetNextConVar(ScriptContext& script_context)
{
    ConVarHandle prevCvar = script_context.GetArgument<ConVarHandle>(0);
    if (!prevCvar.IsValid())
    {
        return -1;
    }

    auto hCvarHandle = globals::cvars->FindNextConVar(prevCvar);
    if (hCvarHandle.IsValid())
    {
        return hCvarHandle.Get();
    }

    return -1;
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

    static INetworkMessageInternal* netMsg = globals::networkMessages->FindNetworkMessagePartial("SetConVar");

    auto msg = netMsg->AllocateMessage()->ToPB<CNETMsg_SetConVar>();
    CMsg_CVars_CVar* cvar = msg->mutable_convars()->add_cvars();
    cvar->set_name(scriptContext.GetArgument<const char*>(1));
    cvar->set_value(scriptContext.GetArgument<const char*>(2));

    pClient->GetNetChannel()->SendNetMessage(msg, BUF_RELIABLE);

    delete msg;
}

REGISTER_NATIVES(commands, {
    ScriptEngine::RegisterNativeHandler("ADD_COMMAND", AddCommand);
    ScriptEngine::RegisterNativeHandler("REMOVE_COMMAND", RemoveCommand);
    ScriptEngine::RegisterNativeHandler("ADD_COMMAND_LISTENER", AddCommandListener);
    ScriptEngine::RegisterNativeHandler("REMOVE_COMMAND_LISTENER", RemoveCommandListener);

    ScriptEngine::RegisterNativeHandler("COMMAND_GET_ARG_COUNT", CommandGetArgCount);
    ScriptEngine::RegisterNativeHandler("COMMAND_GET_ARG_STRING", CommandGetArgString);
    ScriptEngine::RegisterNativeHandler("COMMAND_GET_COMMAND_STRING", CommandGetCommandString);
    ScriptEngine::RegisterNativeHandler("COMMAND_GET_ARG_BY_INDEX", CommandGetArgByIndex);
    ScriptEngine::RegisterNativeHandler("COMMAND_GET_CALLING_CONTEXT", CommandGetCallingContext);

    ScriptEngine::RegisterNativeHandler("FIND_CONVAR", FindConVar);
    ScriptEngine::RegisterNativeHandler("SET_CONVAR_STRING_VALUE", SetConVarStringValue);
    ScriptEngine::RegisterNativeHandler("GET_CONVAR_FROM_HANDLE", GetConVarFromHandle);
    ScriptEngine::RegisterNativeHandler("GET_FIRST_CONVAR", GetFirstConVar);
    ScriptEngine::RegisterNativeHandler("GET_NEXT_CONVAR", GetNextConVar);

    ScriptEngine::RegisterNativeHandler("ISSUE_CLIENT_COMMAND", IssueClientCommand);
    ScriptEngine::RegisterNativeHandler("ISSUE_CLIENT_COMMAND_FROM_SERVER", IssueClientCommandFromServer);
    ScriptEngine::RegisterNativeHandler("REPLICATE_TO_CLIENT", ReplicateToClient);
})
} // namespace counterstrikesharp
