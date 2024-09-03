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
