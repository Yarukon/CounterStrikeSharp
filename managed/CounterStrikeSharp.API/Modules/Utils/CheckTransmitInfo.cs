using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrikeSharp.API.Modules.Utils
{
    public class CheckTransmitInfo : NativeObject
    {
        public CheckTransmitInfo(nint pointer) : base(pointer)
        {
        }

        public unsafe CheckTransmitInfo_Struct** Value => (CheckTransmitInfo_Struct**) this.Handle;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x24C)]
    public struct CheckTransmitInfo_Struct
    {
        [FieldOffset(0)]
        public nint transmitEntity;

        [FieldOffset(8)]
        public nint transmitAlways;

        [FieldOffset(0x248)]
        public int playerSlot;
    }
}
