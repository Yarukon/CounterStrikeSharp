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

public partial class CMoverPathNode : CPointEntity
{
    public CMoverPathNode (IntPtr pointer) : base(pointer) {}

	// m_vInTangentLocal
	[SchemaMember("CMoverPathNode", "m_vInTangentLocal")]
	public Vector InTangentLocal => Schema.GetDeclaredClass<Vector>(this.Handle, "CMoverPathNode", "m_vInTangentLocal");

	// m_vOutTangentLocal
	[SchemaMember("CMoverPathNode", "m_vOutTangentLocal")]
	public Vector OutTangentLocal => Schema.GetDeclaredClass<Vector>(this.Handle, "CMoverPathNode", "m_vOutTangentLocal");

	// m_szParentPathUniqueID
	[SchemaMember("CMoverPathNode", "m_szParentPathUniqueID")]
	public string ParentPathUniqueID
	{
		get { return Schema.GetUtf8String(this.Handle, "CMoverPathNode", "m_szParentPathUniqueID"); }
		set { Schema.SetString(this.Handle, "CMoverPathNode", "m_szParentPathUniqueID", value); }
	}

	// m_OnPassThrough
	[SchemaMember("CMoverPathNode", "m_OnPassThrough")]
	public CEntityIOOutput OnPassThrough => Schema.GetDeclaredClass<CEntityIOOutput>(this.Handle, "CMoverPathNode", "m_OnPassThrough");

	// m_hMover
	[SchemaMember("CMoverPathNode", "m_hMover")]
	public CHandle<CPathMover> Mover => Schema.GetDeclaredClass<CHandle<CPathMover>>(this.Handle, "CMoverPathNode", "m_hMover");

}
