using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrikeSharp.API.Modules.Utils
{
    public enum ParticleAttachment : uint
    {
        PATTACH_INVALID = 0xffffffff,
        PATTACH_ABSORIGIN = 0x0,            // Spawn at entity origin
        PATTACH_ABSORIGIN_FOLLOW = 0x1,     // Spawn at and follow entity origin
        PATTACH_CUSTOMORIGIN = 0x2,
        PATTACH_CUSTOMORIGIN_FOLLOW = 0x3,
        PATTACH_POINT = 0x4,                // Spawn at attachment point
        PATTACH_POINT_FOLLOW = 0x5,         // Spawn at and follow attachment point
        PATTACH_EYES_FOLLOW = 0x6,
        PATTACH_OVERHEAD_FOLLOW = 0x7,
        PATTACH_WORLDORIGIN = 0x8,
        PATTACH_ROOTBONE_FOLLOW = 0x9,
        PATTACH_RENDERORIGIN_FOLLOW = 0xa,
        PATTACH_MAIN_VIEW = 0xb,
        PATTACH_WATERWAKE = 0xc,
        PATTACH_CENTER_FOLLOW = 0xd,
        PATTACH_CUSTOM_GAME_STATE_1 = 0xe,
        PATTACH_HEALTHBAR = 0xf,
        MAX_PATTACH_TYPES = 0x10,
    }
}
