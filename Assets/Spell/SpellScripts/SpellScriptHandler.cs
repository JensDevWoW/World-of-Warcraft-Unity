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

public abstract class SpellScriptHandler : MonoBehaviour
{
    // Called when the spell is cast
    public virtual void OnCast(Unit caster, Unit target)
    {
        // Default behavior (can be empty if there's no default)
    }

    // Called when the spell hits the target
    public virtual void OnHit(Unit caster, Unit target)
    {
        // Default behavior
    }

    // Called when the spell's effects are applied
    public virtual void ApplyEffects(Unit caster, Unit target)
    {
        // Default behavior
    }

    // Called when the spell is interrupted
    public virtual void OnInterrupt(Unit caster, Unit target)
    {
        // Default behavior
    }

    // Called when the spell is finished (whether successful or not)
    public virtual void OnFinish(Unit caster, Unit target)
    {
        // Default behavior
    }
}
