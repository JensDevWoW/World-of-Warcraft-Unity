using UnityEngine;

public class Frostbolt : SpellScript
{
    public override void OnCast(Spell spell, Unit caster, Unit target)
    {
        //Debug.Log($"{caster.name} casts Fireball at {target.name}!");
        // Additional logic for when Fireball is cast
    }

    public override void OnHit(Spell spell, Unit caster, Unit target)
    {
        // Normally we would make sure that the player is frost but since specs don't exist, we will skip that part

        //TODO: Make spec checks
        float percent = 100f;
        if (caster != null)
        {
            float rand = UnityEngine.Random.Range(0, 100);
            if (rand < percent)
            {
                // Add the aura
                caster.CastSpell(16, caster);
            }
        }
    }
}
