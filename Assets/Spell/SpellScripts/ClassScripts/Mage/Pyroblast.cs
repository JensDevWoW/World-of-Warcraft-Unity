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

public class PyroblastScript : SpellScript
{
    public override void OnCast(Spell spell, Unit caster, Unit target)
    {
        // Hot Streak
        if (caster.HasAura(12))
            caster.RemoveAura(12);
    }

    public override void OnHit(Spell spell, Unit caster, Unit target)
    {
        // Null
    }

    public override void Modify(Spell spell, Unit caster, Unit target)
    {
        if (caster.HasAura(12))
            spell.SetCastTime(0);
    }
}
