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
