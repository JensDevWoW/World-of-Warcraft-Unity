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
        spellbook.Add(new SpellList("Drain Soul", 21, KeyBindManager.keyBinds.three));
        spellbook.Add(new SpellList("Agony", 23, KeyBindManager.keyBinds.one));
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

        NetworkClient.Send(msg);
    }
    private bool IsLoggedIn() { return true; }
    private bool CanCast() { return true; }
    private bool IsOnGlobalCooldown() { return false; }
    private bool IsCasting() { return false; }
}
