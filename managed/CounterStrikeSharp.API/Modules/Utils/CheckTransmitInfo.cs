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

        public nint transmitEntity => this.Handle;
        public nint transmitAlways => this.Handle + 0x8;
    }
}
