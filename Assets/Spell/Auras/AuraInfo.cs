using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraInfo
{
    public int Id { get; private set; }

    public string Name { get; private set; }
    public AuraType Type { get; private set; }

    public bool Positive { get; private set; }

    public float Duration { get; private set; }

    public bool Periodic { get; private set; }

    public float TickTime { get; private set; }

    public int Stacks { get; private set; }

    public int BasePoints { get; private set; }

    public string damageClass { get; private set; }

    public string AuraScript { get; private set; }

    public List<AuraEffect> Effects { get; private set; }

    public SpellInfo spellinfo { get; private set; }

    public AuraInfo(int id, string name, AuraType type, bool positive, float duration, bool periodic, float ticktime, int stacks, int basePoints, string damageclass, List<AuraEffect> effects, string aurascript, SpellInfo spellInfo)
    {
        Id = id;
        Name = name;
        Type = type;
        Positive = positive;
        Duration = duration;
        Periodic = periodic;
        TickTime = ticktime;
        Stacks = stacks;
        BasePoints = basePoints;
        damageClass = damageclass;
        Effects = effects;
        AuraScript = aurascript;
        spellinfo = spellInfo;
    }

    public SpellInfo ToSpell()
    {
        return spellinfo;
    }

}

public class AuraType
{
    public string Name { get; private set; }
    public int Id { get; private set; }

    private AuraType(string name, int id)
    {
        Name = name;
        Id = id;
    }

    public static readonly AuraType Buff = new AuraType("Buff", 1);
    public static readonly AuraType Debuff = new AuraType("Debuff", 2);
}