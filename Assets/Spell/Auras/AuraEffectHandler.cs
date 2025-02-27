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

using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AuraEffectHandler
{
    private Dictionary<int, Action<Aura, Unit>> effectHandlers;

    public AuraEffectHandler()
    {
        effectHandlers = new Dictionary<int, Action<Aura, Unit>>();

        // Register handlers for each spell effect
        RegisterHandler(AuraEffect.AURA_EFFECT_DAMAGE, HandleSchoolDamage);
        RegisterHandler(AuraEffect.AURA_EFFECT_APPLY_ABSORB, HandleApplyAbsorb);
        RegisterHandler(AuraEffect.AURA_EFFECT_ROOT, HandleRoot);
        RegisterHandler(AuraEffect.AURA_EFFECT_DISORIENT, HandleDisorient);
        RegisterHandler(AuraEffect.AURA_EFFECT_INCREASE_STAT, HandleStatIncrease);
    }

    private void RegisterHandler(AuraEffect effect, Action<Aura, Unit> handler)
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

    public void HandleEffects(Aura aura)
    {
        Unit target = aura.target;
        // Iterate over the spell's effects and execute the corresponding handlers
        foreach (var effect in aura.auraInfo.Effects)
        {
            if (effectHandlers.TryGetValue(effect.Id, out var handler))
            {
                handler(aura, target);
            }
            else
            {
                Debug.LogWarning($"No handler registered for effect {effect.Name}");
            }
        }
    }

    // Example handler functions for different spell effects
    private void HandleSchoolDamage(Aura aura, Unit target)
    {
        aura.caster.DealDamage(aura.ToSpell(), target);
    }

    private void HandleApplyAbsorb(Aura aura, Unit target)
    {
        target.SetAbsorbAmount(aura.ToSpell().m_spellInfo.BasePoints);
        Debug.Log($"Set Absorb Amount: {target.GetAbsorbAmount()}!");
    }

    private void HandleRoot(Aura aura, Unit target)
    {
        if (!target)
            return;

        // Now it's as simple as adding the unit state
        if (!target.HasUnitState(UnitState.UNIT_STATE_ROOTED))
            target.AddUnitState(UnitState.UNIT_STATE_ROOTED);
    }

    private void HandleDisorient(Aura aura, Unit target)
    {
        if (!target)
            return;

        // Now it's as simple as adding the unit state
        if (!target.HasUnitState(UnitState.UNIT_STATE_DISORIENTED))
            target.AddUnitState(UnitState.UNIT_STATE_DISORIENTED);
    }

    private void HandleStatIncrease(Aura aura, Unit target)
    {
        if (!target)
            return;

        switch (aura.auraInfo.Id)
        {
            case 19: // Arcane Brilliance
                //TODO: Increase a stat
                break;
        }
    }
}
