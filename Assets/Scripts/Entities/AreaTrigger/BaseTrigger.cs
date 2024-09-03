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

using System.Collections.Generic;
using UnityEngine;

public abstract class AreaTriggerScript : MonoBehaviour
{
    // Initialize the script with necessary components
    public void Initialize(Unit caster, AreaTrigger trigger)
    {
        this.caster = caster;
        this.trigger = trigger;
    }

    public abstract void OnUnitEnter(Unit unit);
    public abstract void OnUnitLeave(Unit unit);
    public abstract void IsRemoved(List<Unit> currentUnits);

    protected Unit caster;
    protected AreaTrigger trigger;
}
