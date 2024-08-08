// Filename: GameNetworkManager.cs
using Mirror;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class GameNetworkManager : NetworkManager
{
    private OpcodeHandler opcodeHandler;
    public GameObject spellPrefab;
    public override void OnStartServer()
    {
        opcodeHandler = new OpcodeHandler();

        base.OnStartServer();

        // Register opcode handlers
        opcodeHandler.RegisterHandler(Opcodes.CMSG_CAST_SPELL, HandleCastSpell);
        //opcodeHandler.RegisterHandler((int)Opcodes.MoveCharacter, HandleMoveCharacter);

        NetworkServer.RegisterHandler<OpcodeMessage>(OnOpcodeMessageReceived, true);
    }

    private void OnOpcodeMessageReceived(NetworkConnection conn, OpcodeMessage msg)
    {
        opcodeHandler.HandleOpcode(conn, msg.opcode, new NetworkReader(msg.payload));
    }

    private void HandleCastSpell(NetworkConnection conn, NetworkReader reader)
    {
        int spellId = reader.ReadInt();
        Unit caster = conn.identity.GetComponent<Unit>();

        Spell spell = caster.CreateSpellAndPrepare(spellId, spellPrefab);
    }

    private void HandleMoveCharacter(NetworkConnection conn, NetworkReader reader)
    {
        Vector3 position = reader.ReadVector3();
        Debug.Log($"Server: Move character to {position}");
        // Implement movement logic here
    }
}
