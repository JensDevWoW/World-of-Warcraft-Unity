using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class PlayerInput : NetworkBehaviour
{
    private List<SpellList> spellbook = new List<SpellList>();  // List to store spells
    private bool awaitingAoETarget = false;
    private SpellList currentAoESpell;

    void Start()
    {
        KeyBindManager.LoadKeyBinds();

        spellbook.Add(new SpellList("Flash of Light", 1, KeyBindManager.keyBinds.one));
        spellbook.Add(new SpellList("TestInstant", 3, KeyBindManager.keyBinds.two));
        spellbook.Add(new SpellList("Fireball", 4, KeyBindManager.keyBinds.three));
        spellbook.Add(new SpellList("Power Word: Shield", 5, KeyBindManager.keyBinds.four));
        spellbook.Add(new SpellList("Flamestrike", 6, KeyBindManager.keyBinds.five, true)); // Example AoE spell
    }

    void Update()
    {
        if (!isLocalPlayer || !IsLoggedIn() || !CanCast())
            return;

        if (awaitingAoETarget && Input.GetMouseButtonDown(0))
        {
            // Handle mouse click for AoE spell target position
            Vector3 targetPosition = GetMousePositionOnGround();
            Debug.Log($"Casting AoE {currentAoESpell.Name} with ID {currentAoESpell.SpellId} at position {targetPosition}");
            SendAoESpell(currentAoESpell, targetPosition);
            awaitingAoETarget = false;
            currentAoESpell = null;
        }

        foreach (var spell in spellbook)
        {
            if (Input.GetKeyDown(spell.KeyCode))
            {
                if (spell.IsAoE)
                {
                    awaitingAoETarget = true;
                    currentAoESpell = spell;
                    Debug.Log($"Awaiting target position for {spell.Name}...");
                }
                else
                {
                    Debug.Log($"Casting {spell.Name} with ID {spell.SpellId}");
                    SendOpcode(spell);
                }
            }
        }
    }

    private Vector3 GetMousePositionOnGround()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f); // Draw the ray in the editor for debugging

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            Debug.Log($"Raycast hit: {hit.collider.name} at position {hit.point}");
            return hit.point;
        }
        else
        {
            Debug.LogWarning("Raycast did not hit anything");
        }
        return Vector3.zero;
    }



    private void SendAoESpell(SpellList spell, Vector3 position)
    {
        NetworkWriter writer = new NetworkWriter();

        // Serialize data dynamically
        writer.WriteInt(spell.SpellId);
        writer.WriteString(spell.Name);

        // Write each component of the Vector3 position
        float x = position.x;
        float y = position.y;
        float z = position.z;
        writer.WriteFloat(x);
        writer.WriteFloat(y);
        writer.WriteFloat(z);

        Debug.Log($"Sending AoE Spell: ID={spell.SpellId}, Name={spell.Name}, Position=({x}, {y}, {z})");

        // Create the opcode message
        OpcodeMessage msg = new OpcodeMessage
        {
            opcode = Opcodes.CMSG_CAST_SPELL,
            payload = writer.ToArray()
        };

        // Send the message to the server
        NetworkClient.Send(msg);
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
