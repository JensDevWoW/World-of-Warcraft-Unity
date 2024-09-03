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

public class CooldownHandler
{
    private Dictionary<int, float> cooldowns = new Dictionary<int, float>();

    public void Update()
    {
        List<int> keys = new List<int>(cooldowns.Keys);
        foreach (var key in keys)
        {
            cooldowns[key] -= Time.deltaTime;
            if (cooldowns[key] <= 0)
            {
                cooldowns.Remove(key); // Remove cooldown when it reaches zero
            }
        }
    }

    public void StartCooldown(int spellId, float cooldownDuration)
    {
        if (cooldowns.ContainsKey(spellId))
        {
            cooldowns[spellId] = cooldownDuration; // Reset cooldown
        }
        else
        {
            cooldowns.Add(spellId, cooldownDuration);
        }
    }

    public bool IsCooldownActive(int spellId)
    {
        return cooldowns.ContainsKey(spellId);
    }

    public float GetRemainingCooldown(int spellId)
    {
        return cooldowns.ContainsKey(spellId) ? cooldowns[spellId] : 0;
    }
}
