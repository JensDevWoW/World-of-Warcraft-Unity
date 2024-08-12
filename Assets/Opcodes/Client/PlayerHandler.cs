using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class PlayerInput : NetworkBehaviour
{
    private List<SpellList> spellbook = new List<SpellList>();  // List to store spells

    void Start()
    {
        KeyBindManager.LoadKeyBinds();

        spellbook.Add(new SpellList("Flash of Light", 1, KeyBindManager.keyBinds.one));
        spellbook.Add(new SpellList("TestInstant", 3, KeyBindManager.keyBinds.two));
        spellbook.Add(new SpellList("Fireball", 4, KeyBindManager.keyBinds.three));
        spellbook.Add(new SpellList("Power Word: Shield", 5, KeyBindManager.keyBinds.four));
    }

    void Update()
    {
        if (!isLocalPlayer || !IsLoggedIn() || !CanCast())
            return;

        foreach (var spell in spellbook)
        {
            if (Input.GetKeyDown(spell.KeyCode))
            {
                Debug.Log($"Casting {spell.Name} with ID {spell.SpellId}");
                SendOpcode(spell);
            }
        }
    }

    private void SendOpcode(SpellList spell)
    {
        NetworkWriter writer = new NetworkWriter();

        // Serialize data dynamically
        writer.WriteInt(spell.SpellId);
        writer.WriteString(spell.Name);

        // Create the opcode message
        OpcodeMessage msg = new OpcodeMessage
        {
            opcode = Opcodes.CMSG_CAST_SPELL, // Example opcode, define your opcode logic
            payload = writer.ToArray()
        };

        // Send the message to the server
        CmdSendOpcode(msg);
    }

    [Command]
    private void CmdSendOpcode(OpcodeMessage msg)
    {
        // Send the opcode message to the server
        NetworkClient.Send(msg);
    }

    private bool IsLoggedIn() { return true; }
    private bool CanCast() { return true; }
    private bool IsOnGlobalCooldown() { return false; }
    private bool IsCasting() { return false; }
}
