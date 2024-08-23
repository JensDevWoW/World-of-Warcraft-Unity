// Filename: GameNetworkManager.cs
using Mirror;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class GameNetworkManager : NetworkManager
{
    private OpcodeHandler opcodeHandler;
    public GameObject spellPrefab;
    public GameObject triggerPrefab;
    public override void OnStartServer()
    {
        opcodeHandler = new OpcodeHandler();

        base.OnStartServer();

        // Register opcode handlers
        opcodeHandler.RegisterHandler(Opcodes.CMSG_CAST_SPELL,          HandleCastSpell);
        opcodeHandler.RegisterHandler(Opcodes.CMSG_SELECT_TARGET,       HandleSelectTarget);
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
        Vector3 position = reader.ReadVector3();
        if (position != Vector3.zero)
        {
            caster.CreateSpellAndPrepare(spellId, spellPrefab, triggerPrefab, position);
            return;
        }

        Spell spell = caster.CreateSpellAndPrepare(spellId, spellPrefab, triggerPrefab);
    }


    private void HandleSelectTarget(NetworkConnection conn, NetworkReader reader)
    {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        Unit target = identity.GetComponent<Unit>();

        Unit player = conn.identity.GetComponent<Unit>();

        player.SetTarget(target);
    }
    private void HandleMoveCharacter(NetworkConnection conn, NetworkReader reader)
    {
        Vector3 position = reader.ReadVector3();
        Debug.Log($"Server: Move character to {position}");
        // Implement movement logic here
    }
}
