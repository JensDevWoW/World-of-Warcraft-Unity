/*
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License as published by the
 * Free Software Foundation; either version 2 of the License, or (at your
 * option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
 * more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with this program. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ClientOpcodeHandler
{
    private Dictionary<int, Action<NetworkReader>> opcodeActions;

    public ClientOpcodeHandler()
    {
        opcodeActions = new Dictionary<int, Action<NetworkReader>>();
    }

    public void RegisterHandler(int opcode, Action<NetworkReader> handler)
    {
        if (!opcodeActions.ContainsKey(opcode))
        {
            opcodeActions.Add(opcode, handler);
        }
    }

    public void HandleOpcode(int opcode, NetworkReader reader)
    {
        if (opcodeActions.TryGetValue(opcode, out var handler))
        {
            handler(reader);
        }
        else
        {
            Debug.LogWarning($"No handler registered for opcode: {opcode}");
        }
    }
}
