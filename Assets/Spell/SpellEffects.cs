using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffect
{
    public string Name { get; private set; }
    public int Id { get; private set; }

    private SpellEffect(string name, int id)
    {
        Name = name;
        Id = id;
    }

    public static readonly SpellEffect SPELL_EFFECT_SCHOOL_DAMAGE            = new SpellEffect("SPELL_EFFECT_SCHOOL_DAMAGE", 1);
    public static readonly SpellEffect SPELL_EFFECT_CREATE_AREATRIGGER       = new SpellEffect("SPELL_EFFECT_CREATE_AREATRIGGER", 2);
    public static readonly SpellEffect SPELL_EFFECT_APPLY_AURA               = new SpellEffect("SPELL_EFFECT_APPLY_AURA", 3);
    public static readonly SpellEffect SPELL_EFFECT_DISPEL                   = new SpellEffect("SPELL_EFFECT_DISPEL", 4);
    public static readonly SpellEffect SPELL_EFFECT_TELEPORT                 = new SpellEffect("SPELL_EFFECT_TELEPORT", 5);
    public static readonly SpellEffect SPELL_EFFECT_INTERRUPT_CAST           = new SpellEffect("SPELL_EFFECT_INTERRUPT_CAST", 6);
    public static readonly SpellEffect SPELL_EFFECT_REMOVE_AURA              = new SpellEffect("SPELL_EFFECT_REMOVE_AURA", 7);
    public static readonly SpellEffect SPELL_EFFECT_DUMMY                    = new SpellEffect("SPELL_EFFECT_DUMMY", 8);


    public static SpellEffect operator |(SpellEffect a, SpellEffect b)
    {
        return new SpellEffect($"{a.Name} | {b.Name}", a.Id | b.Id);
    }

    public bool Contains(SpellEffect school)
    {
        return (this.Id & school.Id) != 0;
    }
}
