using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellInfo
{
    public int Id { get; private set; }

    public string Name { get; private set; }    
    public SpellSchoolMask SchoolMask { get; private set; }
    public SpellType Type { get; private set; }
    public int ManaCost { get; private set; }
    public float CastTime { get; private set; }

    public bool SpellTime { get; private set; }

    public float Speed {  get; private set; }

    public bool Positive {  get; private set; }

    public int BasePoints { get; private set; }

    public string damageClass {  get; private set; }

    public string SpellScript { get; private set; }

    public int Cooldown {  get; private set; }

    public List<SpellEffect> Effects { get; private set; }

    public SpellAttributes Attributes { get; private set; }
    public SpellFlags Flags { get; private set; }
    public SpellInfo(int id, string name, SpellSchoolMask schoolMask, SpellType type, int manaCost, float castTime, bool spellTime, float speed, bool positive, int basePoints, string damageclass, List<SpellEffect> effects, string spellScript, int cooldown, SpellAttributes attributes, SpellFlags flags)
    {
        Id = id;
        Name = name;
        SchoolMask = schoolMask;
        Type = type;
        ManaCost = manaCost;
        CastTime = castTime;
        SpellTime = spellTime;
        Speed = speed;
        Positive = positive;
        BasePoints = basePoints;
        damageClass = damageclass;
        Effects = effects;
        SpellScript = spellScript;
        Cooldown = cooldown;
        Attributes = attributes;
        Flags = flags;
    }

    public bool HasFlag(SpellFlags flag)
    {
        if (Flags.HasFlag(flag))
            return true;

        return false;
    }

}


public class SpellSchoolMask
{
    public string Name { get; private set; }
    public int Id { get; private set; }

    private SpellSchoolMask(string name, int id)
    {
        Name = name;
        Id = id;
    }

    public static readonly SpellSchoolMask Physical = new SpellSchoolMask("Physical", 1);
    public static readonly SpellSchoolMask Holy = new SpellSchoolMask("Holy", 2);
    public static readonly SpellSchoolMask Fire = new SpellSchoolMask("Fire", 4);
    public static readonly SpellSchoolMask Nature = new SpellSchoolMask("Nature", 8);
    public static readonly SpellSchoolMask Frost = new SpellSchoolMask("Frost", 16);
    public static readonly SpellSchoolMask Shadow = new SpellSchoolMask("Shadow", 32);
    public static readonly SpellSchoolMask Arcane = new SpellSchoolMask("Arcane", 64);

    public static SpellSchoolMask operator |(SpellSchoolMask a, SpellSchoolMask b)
    {
        return new SpellSchoolMask($"{a.Name} | {b.Name}", a.Id | b.Id);
    }

    public bool Contains(SpellSchoolMask school)
    {
        return (this.Id & school.Id) != 0;
    }
}

public class SpellType
{
    public string Name { get; private set; }
    public int Id { get; private set; }

    private SpellType(string name, int id)
    {
        Name = name;
        Id = id;
    }

    public static readonly SpellType Damage = new SpellType("Damage", 1);
    public static readonly SpellType Heal = new SpellType("Heal", 2);
    public static readonly SpellType Buff = new SpellType("Buff", 3);
    public static readonly SpellType Debuff = new SpellType("Debuff", 4);
    public static readonly SpellType Summon = new SpellType("Summon", 5);
    public static readonly SpellType Teleport = new SpellType("Teleport", 6);
    public static readonly SpellType Control = new SpellType("Control", 7);
    public static readonly SpellType Utility = new SpellType("Utility", 8);
}

public class DamageClass
{
    public string Name { get; private set; }
    public int Id { get; private set; }

    private DamageClass(string name, int id)
    {
        Name = name;
        Id = id;
    }

    public static readonly DamageClass Damage = new DamageClass("Strength", 1);
    public static readonly DamageClass Heal = new DamageClass("Intellect", 2);
    public static readonly DamageClass Buff = new DamageClass("Agility", 3);
    
}