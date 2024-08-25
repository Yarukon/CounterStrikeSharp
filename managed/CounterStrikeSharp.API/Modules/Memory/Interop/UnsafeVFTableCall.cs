using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrikeSharp.API.Modules.Memory.Interop
{
    public class UnsafeVFTableCall
    {
        public static T GetVirtualFunction<T>(nint address, int offset) where T : Delegate => Marshal.GetDelegateForFunctionPointer<T>(GetVirtualFunctionPointer(address, offset));

        public static nint GetVirtualFunctionPointer(nint address, int offset)
        {
            if (address == 0)
                throw new ArgumentException("Invalid address.");

            unsafe
            {
                nint** vftable = *(nint***)address;

                if (vftable == null)
                    throw new ArgumentException("Failed to get the virtual function table.");

                nint addr = (nint) vftable[offset];
                return addr;
            }
        }
    }
}
