using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrikeSharp.API.Modules.Utils
{
    public unsafe class BitVecUtils
    {
        public static void Set(nint address, int index)
        {
            if (address == 0)
                return;

            int i = index >> 5;
            int b = index & 31;
            uint* ptr = (uint*)(address + (i * 4));
            *ptr |= (uint)(1 << b);
        }

        public static void Clear(nint address, int index)
        {
            if (address == 0)
                return;

            int i = index >> 5;
            int b = index & 31;
            uint* ptr = (uint*)(address + (i * 4));
            *ptr &= ~(uint)(1 << b);
        }

        public static bool IsSet(nint address, int index)
        {
            if (address == 0)
                return false;

            int i = index >> 5;
            int b = index & 31;
            uint* ptr = (uint*)(address + (i * 4));
            return (*ptr & (1 << b)) != 0;
        }
    }
}
