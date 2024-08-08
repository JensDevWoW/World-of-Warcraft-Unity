using UnityEngine;

public class FireballScript : SpellScript
{
    public override void OnCast(Unit caster, Unit target)
    {
        Debug.Log($"{caster.name} casts Fireball at {target.name}!");
        // Additional logic for when Fireball is cast
    }

    public override void OnHit(Unit caster, Unit target)
    {
        Debug.Log($"{target.name} is hit by Fireball!");
        // Additional logic for when Fireball hits
        //target.TakeDamage(50); // Example damage application
    }
}
