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

using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellEffectHandler
{
    private Dictionary<int, Action<Spell, Unit>> effectHandlers;

    public SpellEffectHandler()
    {
        effectHandlers = new Dictionary<int, Action<Spell, Unit>>();

        // Register handlers for each spell effect
        RegisterHandler(SpellEffect.SPELL_EFFECT_SCHOOL_DAMAGE, HandleSchoolDamage);
        RegisterHandler(SpellEffect.SPELL_EFFECT_CREATE_AREATRIGGER, HandleCreateAreaTrigger);
        RegisterHandler(SpellEffect.SPELL_EFFECT_APPLY_AURA, HandleApplyAura);
        RegisterHandler(SpellEffect.SPELL_EFFECT_DISPEL, HandleDispel);
        RegisterHandler(SpellEffect.SPELL_EFFECT_TELEPORT, HandleTeleport);
        RegisterHandler(SpellEffect.SPELL_EFFECT_INTERRUPT_CAST, HandleInterruptCast);
        RegisterHandler(SpellEffect.SPELL_EFFECT_REMOVE_AURA, HandleRemoveAura);
        RegisterHandler(SpellEffect.SPELL_EFFECT_DUMMY, HandleDummyEffect);
    }

    private void RegisterHandler(SpellEffect effect, Action<Spell, Unit> handler)
    {
        if (!effectHandlers.ContainsKey(effect.Id))
        {
            effectHandlers.Add(effect.Id, handler);
        }
        else
        {
            Debug.LogWarning($"Effect handler for {effect.Name} is already registered.");
        }
    }

    public void HandleEffects(Spell spell)
    {
        Unit target = spell.target;
        // Iterate over the spell's effects and execute the corresponding handlers
        foreach (var effect in spell.m_spellInfo.Effects)
        {
            if (effectHandlers.TryGetValue(effect.Id, out var handler))
            {
                handler(spell, target);
                spell.m_spellState = Spell.SPELL_STATE_NULL;
            }
            else
            {
                Debug.LogWarning($"No handler registered for effect {effect.Name}");
            }
        }
    }

    // Example handler functions for different spell effects
    private void HandleSchoolDamage(Spell spell, Unit target)
    {
        List<Unit> targets = spell.GetTargets();
        foreach (Unit tar in targets)
        {
            spell.caster.DealDamage(spell, tar);
        }
    }

    private void HandleCreateAreaTrigger(Spell spell, Unit target)
    {
        spell.caster.CreateAreaTriggerAndActivate(spell, target, spell.spellId, spell.trigger);
    }

    private void HandleApplyAura(Spell spell, Unit target)
    {
        if (target == null)
            return;

        Debug.Log($"Applying aura effect from spell {spell.spellId} to target {target.m_name}");
        List<Unit> targets = spell.GetTargets();
        foreach (Unit tar in targets)
        {
            // First check if we already have that aura
            // When you cast the same spell on a target with a particular aura, it should refresh that aura rather than double it
            bool auraReapply = false;
            foreach (Aura aura in tar.GetAuras())
            {
                if (aura.auraInfo.Id == spell.m_spellInfo.Id)
                {
                    aura.Refresh();
                    auraReapply = true;
                    break;
                }
            }

            // This is so we don't do this for every target
            // Some targets might not have the aura already
            if (auraReapply)
                break;


            // Create and initialize the Aura object
            GameObject auraObject = new GameObject("AuraObject");

            Aura newAura = auraObject.AddComponent<Aura>();
            AuraInfo auraInfo = SpellDataHandler.Instance.Auras.FirstOrDefault(aura => aura.Id == spell.spellId);

            auraObject.name = $"Aura: {auraInfo.Id}";

            newAura.Initialize(auraInfo, spell.caster, tar, spell);


            // Send opcode
            NetworkWriter writer = new NetworkWriter();

            writer.WriteNetworkIdentity(spell.caster.Identity);
            writer.WriteNetworkIdentity(tar.Identity);
            writer.WriteInt(auraInfo.Id);
            writer.WriteFloat(auraInfo.Duration);

            OpcodeMessage packet = new OpcodeMessage
            {
                opcode = Opcodes.SMSG_APPLY_AURA,
                payload = writer.ToArray()
            };

            NetworkServer.SendToAll(packet);
        }
    }

    private void HandleDispel(Spell spell, Unit target)
    {
        Debug.Log($"Dispelling effects on target {target.m_name} using spell {spell.spellId}");
        // Implement the logic for dispelling here
    }

    private void HandleTeleport(Spell spell, Unit target)
    {
        // Just going to hard-code this for now
        if (spell.m_spellInfo.Id == 18) // Blink
        {
            target = spell.caster;
            if (target != null)
            {
                Transform tarTran = target.transform;
                if (tarTran != null)
                {
                    target.locationHandler.BlinkEffect(target, tarTran);
                }
            }
        }
    }

    private void HandleInterruptCast(Spell spell, Unit target)
    {
        Debug.Log($"Interrupting cast on target {target.m_name} using spell {spell.spellId}");
        // Implement the logic for interrupting cast here
    }

    private void HandleRemoveAura(Spell spell, Unit target)
    {
        Debug.Log($"Removing aura from target {target.m_name} using spell {spell.spellId}");
        // Implement the logic for removing an aura here
    }

    private void HandleDummyEffect(Spell spell, Unit target)
    {
        // Handled in SpellScripts
    }
}
