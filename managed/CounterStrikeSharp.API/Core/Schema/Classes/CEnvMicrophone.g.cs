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

public partial class CEnvMicrophone : CPointEntity
{
    public CEnvMicrophone (IntPtr pointer) : base(pointer) {}

	// m_bDisabled
	[SchemaMember("CEnvMicrophone", "m_bDisabled")]
	public ref bool Disabled => ref Schema.GetRef<bool>(this.Handle, "CEnvMicrophone", "m_bDisabled");

	// m_hMeasureTarget
	[SchemaMember("CEnvMicrophone", "m_hMeasureTarget")]
	public CHandle<CBaseEntity> MeasureTarget => Schema.GetDeclaredClass<CHandle<CBaseEntity>>(this.Handle, "CEnvMicrophone", "m_hMeasureTarget");

	// m_nSoundType
	[SchemaMember("CEnvMicrophone", "m_nSoundType")]
	public ref SoundTypes_t SoundType => ref Schema.GetRef<SoundTypes_t>(this.Handle, "CEnvMicrophone", "m_nSoundType");

	// m_nSoundFlags
	[SchemaMember("CEnvMicrophone", "m_nSoundFlags")]
	public ref SoundFlags_t SoundFlags => ref Schema.GetRef<SoundFlags_t>(this.Handle, "CEnvMicrophone", "m_nSoundFlags");

	// m_flSensitivity
	[SchemaMember("CEnvMicrophone", "m_flSensitivity")]
	public ref float Sensitivity => ref Schema.GetRef<float>(this.Handle, "CEnvMicrophone", "m_flSensitivity");

	// m_flSmoothFactor
	[SchemaMember("CEnvMicrophone", "m_flSmoothFactor")]
	public ref float SmoothFactor => ref Schema.GetRef<float>(this.Handle, "CEnvMicrophone", "m_flSmoothFactor");

	// m_flMaxRange
	[SchemaMember("CEnvMicrophone", "m_flMaxRange")]
	public ref float MaxRange => ref Schema.GetRef<float>(this.Handle, "CEnvMicrophone", "m_flMaxRange");

	// m_iszSpeakerName
	[SchemaMember("CEnvMicrophone", "m_iszSpeakerName")]
	public string SpeakerName
	{
		get { return Schema.GetUtf8String(this.Handle, "CEnvMicrophone", "m_iszSpeakerName"); }
		set { Schema.SetString(this.Handle, "CEnvMicrophone", "m_iszSpeakerName", value); }
	}

	// m_hSpeaker
	[SchemaMember("CEnvMicrophone", "m_hSpeaker")]
	public CHandle<CBaseEntity> Speaker => Schema.GetDeclaredClass<CHandle<CBaseEntity>>(this.Handle, "CEnvMicrophone", "m_hSpeaker");

	// m_bAvoidFeedback
	[SchemaMember("CEnvMicrophone", "m_bAvoidFeedback")]
	public ref bool AvoidFeedback => ref Schema.GetRef<bool>(this.Handle, "CEnvMicrophone", "m_bAvoidFeedback");

	// m_iSpeakerDSPPreset
	[SchemaMember("CEnvMicrophone", "m_iSpeakerDSPPreset")]
	public ref Int32 SpeakerDSPPreset => ref Schema.GetRef<Int32>(this.Handle, "CEnvMicrophone", "m_iSpeakerDSPPreset");

	// m_iszListenFilter
	[SchemaMember("CEnvMicrophone", "m_iszListenFilter")]
	public string IszListenFilter
	{
		get { return Schema.GetUtf8String(this.Handle, "CEnvMicrophone", "m_iszListenFilter"); }
		set { Schema.SetString(this.Handle, "CEnvMicrophone", "m_iszListenFilter", value); }
	}

	// m_hListenFilter
	[SchemaMember("CEnvMicrophone", "m_hListenFilter")]
	public CHandle<CBaseFilter> HListenFilter => Schema.GetDeclaredClass<CHandle<CBaseFilter>>(this.Handle, "CEnvMicrophone", "m_hListenFilter");

	// m_OnRoutedSound
	[SchemaMember("CEnvMicrophone", "m_OnRoutedSound")]
	public CEntityIOOutput OnRoutedSound => Schema.GetDeclaredClass<CEntityIOOutput>(this.Handle, "CEnvMicrophone", "m_OnRoutedSound");

	// m_OnHeardSound
	[SchemaMember("CEnvMicrophone", "m_OnHeardSound")]
	public CEntityIOOutput OnHeardSound => Schema.GetDeclaredClass<CEntityIOOutput>(this.Handle, "CEnvMicrophone", "m_OnHeardSound");

	// m_szLastSound
	[SchemaMember("CEnvMicrophone", "m_szLastSound")]
	public string LastSound
	{
		get { return Schema.GetString(this.Handle, "CEnvMicrophone", "m_szLastSound"); }
		set { Schema.SetStringBytes(this.Handle, "CEnvMicrophone", "m_szLastSound", value, 256); }
	}

	// m_iLastRoutedFrame
	[SchemaMember("CEnvMicrophone", "m_iLastRoutedFrame")]
	public ref Int32 LastRoutedFrame => ref Schema.GetRef<Int32>(this.Handle, "CEnvMicrophone", "m_iLastRoutedFrame");

}
