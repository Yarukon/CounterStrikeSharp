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

public partial class CCitadelSoundOpvarSetOBB : CBaseEntity
{
    public CCitadelSoundOpvarSetOBB (IntPtr pointer) : base(pointer) {}

	// m_iszStackName
	[SchemaMember("CCitadelSoundOpvarSetOBB", "m_iszStackName")]
	public string StackName
	{
		get { return Schema.GetUtf8String(this.Handle, "CCitadelSoundOpvarSetOBB", "m_iszStackName"); }
		set { Schema.SetString(this.Handle, "CCitadelSoundOpvarSetOBB", "m_iszStackName", value); }
	}

	// m_iszOperatorName
	[SchemaMember("CCitadelSoundOpvarSetOBB", "m_iszOperatorName")]
	public string OperatorName
	{
		get { return Schema.GetUtf8String(this.Handle, "CCitadelSoundOpvarSetOBB", "m_iszOperatorName"); }
		set { Schema.SetString(this.Handle, "CCitadelSoundOpvarSetOBB", "m_iszOperatorName", value); }
	}

	// m_iszOpvarName
	[SchemaMember("CCitadelSoundOpvarSetOBB", "m_iszOpvarName")]
	public string OpvarName
	{
		get { return Schema.GetUtf8String(this.Handle, "CCitadelSoundOpvarSetOBB", "m_iszOpvarName"); }
		set { Schema.SetString(this.Handle, "CCitadelSoundOpvarSetOBB", "m_iszOpvarName", value); }
	}

	// m_vDistanceInnerMins
	[SchemaMember("CCitadelSoundOpvarSetOBB", "m_vDistanceInnerMins")]
	public Vector DistanceInnerMins => Schema.GetDeclaredClass<Vector>(this.Handle, "CCitadelSoundOpvarSetOBB", "m_vDistanceInnerMins");

	// m_vDistanceInnerMaxs
	[SchemaMember("CCitadelSoundOpvarSetOBB", "m_vDistanceInnerMaxs")]
	public Vector DistanceInnerMaxs => Schema.GetDeclaredClass<Vector>(this.Handle, "CCitadelSoundOpvarSetOBB", "m_vDistanceInnerMaxs");

	// m_vDistanceOuterMins
	[SchemaMember("CCitadelSoundOpvarSetOBB", "m_vDistanceOuterMins")]
	public Vector DistanceOuterMins => Schema.GetDeclaredClass<Vector>(this.Handle, "CCitadelSoundOpvarSetOBB", "m_vDistanceOuterMins");

	// m_vDistanceOuterMaxs
	[SchemaMember("CCitadelSoundOpvarSetOBB", "m_vDistanceOuterMaxs")]
	public Vector DistanceOuterMaxs => Schema.GetDeclaredClass<Vector>(this.Handle, "CCitadelSoundOpvarSetOBB", "m_vDistanceOuterMaxs");

	// m_nAABBDirection
	[SchemaMember("CCitadelSoundOpvarSetOBB", "m_nAABBDirection")]
	public ref Int32 AABBDirection => ref Schema.GetRef<Int32>(this.Handle, "CCitadelSoundOpvarSetOBB", "m_nAABBDirection");

}