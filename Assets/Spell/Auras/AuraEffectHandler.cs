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
        RegisterHandler(AuraEffect.AURA_EFFECT_DUMMY, HandleDummyEffect);
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

    private void HandleDummyEffect(Aura aura, Unit target)
    {
        Debug.Log($"Executing dummy effect from aura {aura.auraInfo.Id} on target {target.m_name}");
        // Implement the logic for dummy effect here
    }
}
