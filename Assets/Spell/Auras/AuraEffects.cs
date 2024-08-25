using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraEffect
{
    public string Name { get; private set; }
    public int Id { get; private set; }

    private AuraEffect(string name, int id)
    {
        Name = name;
        Id = id;
    }

    public static readonly AuraEffect AURA_EFFECT_DAMAGE = new AuraEffect("AURA_EFFECT_DAMAGE", 1);
    public static readonly AuraEffect AURA_EFFECT_APPLY_ABSORB = new AuraEffect("AURA_EFFECT_APPLY_ABSORB", 2);
    public static readonly AuraEffect AURA_EFFECT_ROOT = new AuraEffect("AURA_EFFECT_ROOT", 4);
    public static readonly AuraEffect AURA_EFFECT_DUMMY = new AuraEffect("AURA_EFFECT_DUMMY", 100);


    public static AuraEffect operator |(AuraEffect a, AuraEffect b)
    {
        return new AuraEffect($"{a.Name} | {b.Name}", a.Id | b.Id);
    }

    public bool Contains(AuraEffect school)
    {
        return (this.Id & school.Id) != 0;
    }
}
