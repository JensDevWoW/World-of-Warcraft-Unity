using UnityEngine;

public class IceLance : SpellScript
{
    public override void OnCast(Spell spell, Unit caster, Unit target)
    {
        //Debug.Log($"{caster.name} casts Fireball at {target.name}!");
        // Additional logic for when Fireball is cast
    }

    public override void OnHit(Spell spell, Unit caster, Unit target)
    {
        if (caster != null && caster.HasAura(16)) // Finger's of Frost
            caster.RemoveAura(16);
    }

    public override void Modify(Spell spell, Unit caster, Unit target)
    {
        if (!caster)
            return;

        if (caster.HasAura(16)) // Finger's of Frost Damage Increase
            spell.ModBasePoints(1.5f);
    }
}
