// Filename: OpcodeMessage.cs
using Mirror;

public struct OpcodeMessage : NetworkMessage
{
    public int opcode;
    public byte[] payload;
}
