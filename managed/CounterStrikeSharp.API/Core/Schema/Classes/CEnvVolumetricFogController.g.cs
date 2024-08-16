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

public partial class CEnvVolumetricFogController : CBaseEntity
{
    public CEnvVolumetricFogController (IntPtr pointer) : base(pointer) {}

	// m_flScattering
	[SchemaMember("CEnvVolumetricFogController", "m_flScattering")]
	public ref float Scattering => ref Schema.GetRef<float>(this.Handle, "CEnvVolumetricFogController", "m_flScattering");

	// m_flAnisotropy
	[SchemaMember("CEnvVolumetricFogController", "m_flAnisotropy")]
	public ref float Anisotropy => ref Schema.GetRef<float>(this.Handle, "CEnvVolumetricFogController", "m_flAnisotropy");

	// m_flFadeSpeed
	[SchemaMember("CEnvVolumetricFogController", "m_flFadeSpeed")]
	public ref float FadeSpeed => ref Schema.GetRef<float>(this.Handle, "CEnvVolumetricFogController", "m_flFadeSpeed");

	// m_flDrawDistance
	[SchemaMember("CEnvVolumetricFogController", "m_flDrawDistance")]
	public ref float DrawDistance => ref Schema.GetRef<float>(this.Handle, "CEnvVolumetricFogController", "m_flDrawDistance");

	// m_flFadeInStart
	[SchemaMember("CEnvVolumetricFogController", "m_flFadeInStart")]
	public ref float FadeInStart => ref Schema.GetRef<float>(this.Handle, "CEnvVolumetricFogController", "m_flFadeInStart");

	// m_flFadeInEnd
	[SchemaMember("CEnvVolumetricFogController", "m_flFadeInEnd")]
	public ref float FadeInEnd => ref Schema.GetRef<float>(this.Handle, "CEnvVolumetricFogController", "m_flFadeInEnd");

	// m_flIndirectStrength
	[SchemaMember("CEnvVolumetricFogController", "m_flIndirectStrength")]
	public ref float IndirectStrength => ref Schema.GetRef<float>(this.Handle, "CEnvVolumetricFogController", "m_flIndirectStrength");

	// m_nIndirectTextureDimX
	[SchemaMember("CEnvVolumetricFogController", "m_nIndirectTextureDimX")]
	public ref Int32 IndirectTextureDimX => ref Schema.GetRef<Int32>(this.Handle, "CEnvVolumetricFogController", "m_nIndirectTextureDimX");

	// m_nIndirectTextureDimY
	[SchemaMember("CEnvVolumetricFogController", "m_nIndirectTextureDimY")]
	public ref Int32 IndirectTextureDimY => ref Schema.GetRef<Int32>(this.Handle, "CEnvVolumetricFogController", "m_nIndirectTextureDimY");

	// m_nIndirectTextureDimZ
	[SchemaMember("CEnvVolumetricFogController", "m_nIndirectTextureDimZ")]
	public ref Int32 IndirectTextureDimZ => ref Schema.GetRef<Int32>(this.Handle, "CEnvVolumetricFogController", "m_nIndirectTextureDimZ");

	// m_vBoxMins
	[SchemaMember("CEnvVolumetricFogController", "m_vBoxMins")]
	public Vector BoxMins => Schema.GetDeclaredClass<Vector>(this.Handle, "CEnvVolumetricFogController", "m_vBoxMins");

	// m_vBoxMaxs
	[SchemaMember("CEnvVolumetricFogController", "m_vBoxMaxs")]
	public Vector BoxMaxs => Schema.GetDeclaredClass<Vector>(this.Handle, "CEnvVolumetricFogController", "m_vBoxMaxs");

	// m_bActive
	[SchemaMember("CEnvVolumetricFogController", "m_bActive")]
	public ref bool Active => ref Schema.GetRef<bool>(this.Handle, "CEnvVolumetricFogController", "m_bActive");

	// m_flStartAnisoTime
	[SchemaMember("CEnvVolumetricFogController", "m_flStartAnisoTime")]
	public ref float StartAnisoTime => ref Schema.GetRef<float>(this.Handle, "CEnvVolumetricFogController", "m_flStartAnisoTime");

	// m_flStartScatterTime
	[SchemaMember("CEnvVolumetricFogController", "m_flStartScatterTime")]
	public ref float StartScatterTime => ref Schema.GetRef<float>(this.Handle, "CEnvVolumetricFogController", "m_flStartScatterTime");

	// m_flStartDrawDistanceTime
	[SchemaMember("CEnvVolumetricFogController", "m_flStartDrawDistanceTime")]
	public ref float StartDrawDistanceTime => ref Schema.GetRef<float>(this.Handle, "CEnvVolumetricFogController", "m_flStartDrawDistanceTime");

	// m_flStartAnisotropy
	[SchemaMember("CEnvVolumetricFogController", "m_flStartAnisotropy")]
	public ref float StartAnisotropy => ref Schema.GetRef<float>(this.Handle, "CEnvVolumetricFogController", "m_flStartAnisotropy");

	// m_flStartScattering
	[SchemaMember("CEnvVolumetricFogController", "m_flStartScattering")]
	public ref float StartScattering => ref Schema.GetRef<float>(this.Handle, "CEnvVolumetricFogController", "m_flStartScattering");

	// m_flStartDrawDistance
	[SchemaMember("CEnvVolumetricFogController", "m_flStartDrawDistance")]
	public ref float StartDrawDistance => ref Schema.GetRef<float>(this.Handle, "CEnvVolumetricFogController", "m_flStartDrawDistance");

	// m_flDefaultAnisotropy
	[SchemaMember("CEnvVolumetricFogController", "m_flDefaultAnisotropy")]
	public ref float DefaultAnisotropy => ref Schema.GetRef<float>(this.Handle, "CEnvVolumetricFogController", "m_flDefaultAnisotropy");

	// m_flDefaultScattering
	[SchemaMember("CEnvVolumetricFogController", "m_flDefaultScattering")]
	public ref float DefaultScattering => ref Schema.GetRef<float>(this.Handle, "CEnvVolumetricFogController", "m_flDefaultScattering");

	// m_flDefaultDrawDistance
	[SchemaMember("CEnvVolumetricFogController", "m_flDefaultDrawDistance")]
	public ref float DefaultDrawDistance => ref Schema.GetRef<float>(this.Handle, "CEnvVolumetricFogController", "m_flDefaultDrawDistance");

	// m_bStartDisabled
	[SchemaMember("CEnvVolumetricFogController", "m_bStartDisabled")]
	public ref bool StartDisabled => ref Schema.GetRef<bool>(this.Handle, "CEnvVolumetricFogController", "m_bStartDisabled");

	// m_bEnableIndirect
	[SchemaMember("CEnvVolumetricFogController", "m_bEnableIndirect")]
	public ref bool EnableIndirect => ref Schema.GetRef<bool>(this.Handle, "CEnvVolumetricFogController", "m_bEnableIndirect");

	// m_bIndirectUseLPVs
	[SchemaMember("CEnvVolumetricFogController", "m_bIndirectUseLPVs")]
	public ref bool IndirectUseLPVs => ref Schema.GetRef<bool>(this.Handle, "CEnvVolumetricFogController", "m_bIndirectUseLPVs");

	// m_bIsMaster
	[SchemaMember("CEnvVolumetricFogController", "m_bIsMaster")]
	public ref bool IsMaster => ref Schema.GetRef<bool>(this.Handle, "CEnvVolumetricFogController", "m_bIsMaster");

	// m_hFogIndirectTexture
	[SchemaMember("CEnvVolumetricFogController", "m_hFogIndirectTexture")]
	public CStrongHandle<InfoForResourceTypeCTextureBase> FogIndirectTexture => Schema.GetDeclaredClass<CStrongHandle<InfoForResourceTypeCTextureBase>>(this.Handle, "CEnvVolumetricFogController", "m_hFogIndirectTexture");

	// m_nForceRefreshCount
	[SchemaMember("CEnvVolumetricFogController", "m_nForceRefreshCount")]
	public ref Int32 ForceRefreshCount => ref Schema.GetRef<Int32>(this.Handle, "CEnvVolumetricFogController", "m_nForceRefreshCount");

	// m_fNoiseSpeed
	[SchemaMember("CEnvVolumetricFogController", "m_fNoiseSpeed")]
	public ref float NoiseSpeed => ref Schema.GetRef<float>(this.Handle, "CEnvVolumetricFogController", "m_fNoiseSpeed");

	// m_fNoiseStrength
	[SchemaMember("CEnvVolumetricFogController", "m_fNoiseStrength")]
	public ref float NoiseStrength => ref Schema.GetRef<float>(this.Handle, "CEnvVolumetricFogController", "m_fNoiseStrength");

	// m_vNoiseScale
	[SchemaMember("CEnvVolumetricFogController", "m_vNoiseScale")]
	public Vector NoiseScale => Schema.GetDeclaredClass<Vector>(this.Handle, "CEnvVolumetricFogController", "m_vNoiseScale");

	// m_bFirstTime
	[SchemaMember("CEnvVolumetricFogController", "m_bFirstTime")]
	public ref bool FirstTime => ref Schema.GetRef<bool>(this.Handle, "CEnvVolumetricFogController", "m_bFirstTime");

}
