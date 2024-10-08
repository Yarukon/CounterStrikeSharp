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

public partial class CPointChildModifier : CPointEntity
{
    public CPointChildModifier (IntPtr pointer) : base(pointer) {}

	// m_bOrphanInsteadOfDeletingChildrenOnRemove
	[SchemaMember("CPointChildModifier", "m_bOrphanInsteadOfDeletingChildrenOnRemove")]
	public ref bool OrphanInsteadOfDeletingChildrenOnRemove => ref Schema.GetRef<bool>(this.Handle, "CPointChildModifier", "m_bOrphanInsteadOfDeletingChildrenOnRemove");

}
