using System;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Core;

public partial class CBaseEntity
{
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    /// <exception cref="ArgumentNullException">No valid argument</exception>
    public void Teleport(Vector? position = null, QAngle? angles = null, Vector? velocity = null)
    {
        Guard.IsValidEntity(this);
    
        if (position == null && angles == null && velocity == null)
            throw new ArgumentNullException("No valid argument");
    
        nint _position = position?.Handle ?? 0;
        nint _angles = angles?.Handle ?? 0;
        nint _velocity = velocity?.Handle ?? 0;
    
        VirtualFunction.CreateVoid<IntPtr, IntPtr, IntPtr, IntPtr>(Handle, GameData.GetOffset("CBaseEntity_Teleport"))(Handle, _position, _angles, _velocity);
    }

    /// <summary>Compatibility function for those old plugins</summary>
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public void DispatchSpawn() => DispatchSpawn(null);

    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public unsafe void DispatchSpawn(CEntityKeyValues? keyValues)
    {
        Guard.IsValidEntity(this);

        if (keyValues != null && keyValues.Count() > 0)
        {
            KeyValuesEntry** entries = stackalloc KeyValuesEntry*[keyValues.Count()];
            keyValues.Build(entries);
            NativeAPI.DispatchSpawn(this.Handle, keyValues.Count(), (nint)entries);
            keyValues.Free();
        }
        else
            NativeAPI.DispatchSpawn(this.Handle, 0, 0);
    }

    /// <summary>
    /// Shorthand for accessing an entity's CBodyComponent?.SceneNode?.AbsOrigin;
    /// </summary>
    public Vector? AbsOrigin => CBodyComponent?.SceneNode?.AbsOrigin;

    /// <summary>
    /// Shorthand for accessing an entity's CBodyComponent?.SceneNode?.AbsRotation;
    /// </summary>
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public QAngle? AbsRotation => CBodyComponent?.SceneNode?.AbsRotation;

    public T? GetVData<T>() where T : CEntitySubclassVDataBase
    {
        Guard.IsValidEntity(this);

        return (T)Activator.CreateInstance(typeof(T), Marshal.ReadIntPtr(SubclassID.Handle + 4));
    }

    /// <summary>
    /// Plays a sound on an entity.
    /// </summary>
    /// <param name="soundName">The sound to play. This should be a sound script name, file path is not supported.</param>
    /// <param name="pitch">The pitch applied to the sound. 1.0 means the pitch is not changed.</param>
    /// <param name="volume">The volume of the sound.</param>
    /// <param name="filter">If set, the sound will only be networked to the clients in the filter.</param>
    /// <exception cref="InvalidOperationException">Invalid entity</exception>
    public void EmitSound(string soundName, float pitch = 1f, float volume = 1f, CRecipientFilter? filter = null)
    {
        Guard.IsValidEntity(this);

        if (filter != null)
            NativeAPI.EmitSound(this.Index, soundName, pitch, volume, true, filter.GetRecipients());
        else
            NativeAPI.EmitSound(this.Index, soundName, pitch, volume, false, 0);
    }


    /// <summary>
    /// Dispatch particle
    /// </summary>
    /// <param name="particleName">The resource path to particle</param>
    /// <param name="filter">Which player can see the particle</param>
    /// <param name="attachType">Particle attachment type</param>
    /// <param name="attachmentPoint">Particle attachment point</param>
    /// <param name="attachmentName">Attachment name</param>
    public void DispatchParticle(string particleName, CRecipientFilter filter, ParticleAttachment attachType = ParticleAttachment.PATTACH_POINT_FOLLOW, byte attachmentPoint = 0, string attachmentName = "")
    {
        NativeAPI.DispatchParticle(this.Handle, particleName, filter.GetRecipients(), (uint)attachType, attachmentPoint, attachmentName);
    }

    /// <summary>
    /// Dispatch particle
    /// </summary>
    /// <param name="particleName">The resource path to particle</param>
    /// <param name="filter">Which player can see the particle</param>
    /// <param name="origin">Particle render local origin</param>
    /// <param name="angle">Particle render angle</param>
    public void DispatchParticle(string particleName, CRecipientFilter filter, Vector origin, QAngle angle)
    {
        NativeAPI.DispatchParticle2(this.Handle, particleName, filter.GetRecipients(), origin.Handle, angle.Handle);
    }
}
