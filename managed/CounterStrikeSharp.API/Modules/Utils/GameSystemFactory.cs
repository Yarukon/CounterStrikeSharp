using CounterStrikeSharp.API.Modules.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrikeSharp.API.Modules.Utils
{
    public unsafe class GameSystemFactory
    {
        /// <summary>
        /// Dump GameSystems
        /// </summary>
        public static void Dump()
        {
            CBaseGameSystem* pFirst = (CBaseGameSystem*) NativeAPI.GetFirstGamesystemPtr();

            Application.Instance.Logger.LogInformation("-------- G A M E  S Y S T E M S --------");
            Application.Instance.Logger.LogInformation("Address | Name");
            while (pFirst != null)
            {
                string? _name = pFirst->GetName();
                Application.Instance.Logger.LogInformation($"{pFirst->m_pInstance:X12} | {_name ?? "<unnamed>"}");
                pFirst = pFirst->m_pNext;
            }
        }

        /// <summary>
        /// Get GameSystem by name
        /// </summary>
        /// <param name="name">GameSystem name</param>
        /// <exception cref="ArgumentNullException">Name can't be null/empty/whitespace!</exception>
        public static nint Find(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("Name can't be null/empty/whitespace!");

            CBaseGameSystem* pFirst = (CBaseGameSystem*) NativeAPI.GetFirstGamesystemPtr();

            while (pFirst != null)
            {
                string? _name = pFirst->GetName();
                if (_name != null && _name == name)
                    return pFirst->m_pInstance;

                pFirst = pFirst->m_pNext;
            }

            return 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct CBaseGameSystem
        {
            public nint vftable;
            public CBaseGameSystem* m_pNext;
            private nint m_pName;
            public nint m_pInstance;

            public string? GetName() => Marshal.PtrToStringAnsi(m_pName);
        }
    }
}
