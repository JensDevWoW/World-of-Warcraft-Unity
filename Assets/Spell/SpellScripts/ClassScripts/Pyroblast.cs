using UnityEngine;

public class PyroblastScript : SpellScript
{
    public override void OnCast(Spell spell, Unit caster, Unit target)
    {
        Debug.Log($"{caster.name} casts Fireball at {target.name}!");
        // Additional logic for when Fireball is cast
    }

    public override void OnHit(Spell spell, Unit caster, Unit target)
    {
        // Hot Streak
        if (caster.HasAura(12))
            caster.RemoveAura(12);
    }

    public override void Modify(Spell spell, Unit caster, Unit target)
    {
        if (caster.HasAura(12))
            spell.SetCastTime(0);
    }
}
