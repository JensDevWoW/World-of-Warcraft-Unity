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
