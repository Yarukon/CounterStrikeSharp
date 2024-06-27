﻿/*
 *  This file is part of CounterStrikeSharp.
 *  CounterStrikeSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  CounterStrikeSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
 */

using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory;
using System.Runtime.InteropServices;

namespace CounterStrikeSharp.API.Core;

public partial class CCSGameRules
{
    private static readonly bool IsWindowsPlatform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    /// <summary>
    /// Terminate the round with the given delay and reason.
    /// </summary>
    public void TerminateRound(float delay, RoundEndReason roundEndReason)
    {
        if (IsWindowsPlatform)
            VirtualFunctions.TerminateRound(Handle, delay, roundEndReason, 0, 0);
        else
            VirtualFunctions.TerminateRoundLinux(Handle, roundEndReason, 0, 0, delay);
    }
}
