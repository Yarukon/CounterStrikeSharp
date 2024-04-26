// <auto-generated />
#nullable enable
#pragma warning disable CS1591

using System;
using System.Diagnostics;
using System.Drawing;
using CounterStrikeSharp;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

public partial class CCSPlayer_UseServices : CPlayer_UseServices
{
    public CCSPlayer_UseServices (IntPtr pointer) : base(pointer) {}

	// m_hLastKnownUseEntity
	[SchemaMember("CCSPlayer_UseServices", "m_hLastKnownUseEntity")]
	public CHandle<CBaseEntity> LastKnownUseEntity => Schema.GetDeclaredClass<CHandle<CBaseEntity>>(this.Handle, "CCSPlayer_UseServices", "m_hLastKnownUseEntity");

	// m_flLastUseTimeStamp
	[SchemaMember("CCSPlayer_UseServices", "m_flLastUseTimeStamp")]
	public ref float LastUseTimeStamp => ref Schema.GetRef<float>(this.Handle, "CCSPlayer_UseServices", "m_flLastUseTimeStamp");

	// m_flTimeLastUsedWindow
	[SchemaMember("CCSPlayer_UseServices", "m_flTimeLastUsedWindow")]
	public ref float TimeLastUsedWindow => ref Schema.GetRef<float>(this.Handle, "CCSPlayer_UseServices", "m_flTimeLastUsedWindow");

}
