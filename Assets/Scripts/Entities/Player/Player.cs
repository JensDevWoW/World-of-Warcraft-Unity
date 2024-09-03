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

public class Player : MonoBehaviour
{
    public string playerName;
    public int level;

    private float gcdTimer = 0;
    private float gcdTime = 1.5f;

    public Unit unit { get; set; }

    public void Start()
    {
        unit = gameObject.GetComponent<Unit>();
    }

    public void StopCasting()
    {
        unit.StopCasting(); // Optionally call the base class implementation
        Debug.Log($"{playerName} has stopped casting.");
        // Add any additional logic specific to the Player class here
    }

    public Unit ToUnit()
    {
        return unit;
    }

    public bool IsOnGCD()
    {
        return gcdTimer > 0;
    }

    public void SetOnGCD()
    {
        gcdTimer = gcdTime;
    }

    public float GetGCDTime()
    {
        return gcdTimer;
    }
    public void Update()
    {
        // Only players have GCD so update in Player object
        // TODO: Implement haste increase
        if (gcdTimer > 0)
            gcdTimer -= Time.deltaTime;
        else
            gcdTimer = 0;
    }
}
