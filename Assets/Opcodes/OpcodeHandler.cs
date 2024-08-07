// Filename: OpcodeHandler.cs
using System;
using System.Collections.Generic;
using Mirror;

public class OpcodeHandler
{
    private Dictionary<int, Action<NetworkConnection, NetworkReader>> opcodeActions;

    public OpcodeHandler()
    {
        opcodeActions = new Dictionary<int, Action<NetworkConnection, NetworkReader>>();
    }

    public void RegisterHandler(int opcode, Action<NetworkConnection, NetworkReader> handler)
    {
        if (!opcodeActions.ContainsKey(opcode))
        {
            opcodeActions.Add(opcode, handler);
        }
    }

    public void HandleOpcode(NetworkConnection conn, int opcode, NetworkReader reader)
    {
        if (opcodeActions.TryGetValue(opcode, out var handler))
        {
            handler(conn, reader);
        }
        else
        {
            UnityEngine.Debug.LogWarning($"No handler registered for opcode: {opcode}");
        }
    }
}
