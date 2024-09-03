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
