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
    public static readonly AuraEffect AURA_EFFECT_DISORIENT = new AuraEffect("AURA_EFFECT_DISORIENT", 5);
    public static readonly AuraEffect AURA_EFFECT_INCREASE_STAT = new AuraEffect("AURA_EFFECT_INCREASE_STAT", 6);
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
