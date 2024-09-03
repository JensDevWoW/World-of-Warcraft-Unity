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

public class SpellList
{
    public string Name { get; }
    public int SpellId { get; }
    public KeyCode KeyCode { get; }
    public bool IsAoE { get; } // Add this property to identify AoE spells

    public Vector3 Position;
    public SpellList(string name, int spellId, KeyCode keyCode, bool isAoE = false, Vector3 position = default)
    {
        Name = name;
        SpellId = spellId;
        KeyCode = keyCode;
        IsAoE = isAoE;
        Position = position;
    }
}
