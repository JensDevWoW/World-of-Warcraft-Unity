// Filename: GameNetworkManager.cs
using Mirror;
using System.Linq;
using UnityEngine;

public class GameNetworkManager : NetworkManager
{
    private OpcodeHandler opcodeHandler;

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
        Unit target = caster.GetTarget();
        SpellInfo info = SpellDataHandler.Instance.Spells.FirstOrDefault(spell => spell.Id == spellId);
        Spell new_spell = new Spell(spellId, caster, target, info);
        new_spell.prepare();
        // Implement spell casting logic here
    }

    private void HandleMoveCharacter(NetworkConnection conn, NetworkReader reader)
    {
        Vector3 position = reader.ReadVector3();
        Debug.Log($"Server: Move character to {position}");
        // Implement movement logic here
    }
}
