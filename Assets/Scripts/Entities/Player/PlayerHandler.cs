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
        spellbook.Add(new SpellList("Fireball", 4, KeyBindManager.keyBinds.three));
        spellbook.Add(new SpellList("Pyroblast", 13, KeyBindManager.keyBinds.two));
        spellbook.Add(new SpellList("Flamestrike", 6, KeyBindManager.keyBinds.one, true)); // AoE click-placement spell
        spellbook.Add(new SpellList("Combustion", 14, KeyBindManager.keyBinds.four));
        spellbook.Add(new SpellList("Frostbolt", 2, KeyBindManager.keyBinds.five));
        spellbook.Add(new SpellList("Ice Lance", 15, KeyBindManager.keyBinds.six));
        spellbook.Add(new SpellList("Frost Nova", 17, KeyBindManager.keyBinds.seven));
        spellbook.Add(new SpellList("Blink", 18, KeyBindManager.keyBinds.eight));
        spellbook.Add(new SpellList("Arcane Brilliance", 19, KeyBindManager.keyBinds.nine));
    }

    void Update()
    {
        if (!isLocalPlayer || !IsLoggedIn() || !CanCast())
            return;

        if (awaitingAoETarget && Input.GetMouseButtonDown(0))
        {
            // Handle mouse click for AoE spell target position
            Vector3 position = GetMousePositionOnGround();
            currentAoESpell.Position = position;
            SendOpcode(currentAoESpell);
            awaitingAoETarget = false;
            currentAoESpell = null;
            return;
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
                    print($"Casting spell ID: {spell.SpellId}");
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



    private void SendOpcode(SpellList spell)
    {
        NetworkWriter writer = new NetworkWriter();

        writer.WriteInt(spell.SpellId);
        writer.WriteVector3(spell.Position);

        OpcodeMessage msg = new OpcodeMessage
        {
            opcode = Opcodes.CMSG_CAST_SPELL,
            payload = writer.ToArray()
        };

        CmdSendOpcode(msg);
    }


    [Command]
    private void CmdSendOpcode(OpcodeMessage msg)
    {
        NetworkClient.Send(msg);
    }

    private bool IsLoggedIn() { return true; }
    private bool CanCast() { return true; }
    private bool IsOnGlobalCooldown() { return false; }
    private bool IsCasting() { return false; }
}
