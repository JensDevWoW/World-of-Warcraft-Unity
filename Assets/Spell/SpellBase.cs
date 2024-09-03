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

public class SpellBase : MonoBehaviour
{
    protected Transform caster;
    protected Transform target;
    protected float speed;

    public virtual void Initialize(Transform caster, Transform target, float Speed)
    {
        this.caster = caster;
        this.target = target;
        this.speed = Speed;
        this.enabled = false; // Start disabled, will be enabled when the spell is cast
    }

    protected virtual void Update()
    {
        // Implement spell movement and logic here, e.g., move towards the target
        // Override this method in derived classes for different behaviors
    }
}
