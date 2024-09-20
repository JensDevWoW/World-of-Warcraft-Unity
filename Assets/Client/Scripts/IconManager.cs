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

public class IconManager : MonoBehaviour
{
    public static Sprite GetSpellIcon(string className, int spellId)
    {
        // Construct the path to the sprite in the Resources folder
        string path = $"SpellIcons/{className}/" + spellId;

        // Load the sprite from the Resources folder
        Sprite spellIcon = Resources.Load<Sprite>(path);

        // Check if the sprite was successfully loaded
        if (spellIcon == null)
        {
            Debug.LogWarning("Spell icon not found for spellId: " + spellId);
        }

        return spellIcon;
    }

    public static Sprite GetSpellIcon(int spellId)
    {
        // Define the base path where all spell icons are located
        string basePath = "SpellIcons/";

        // Construct the full path to the sprite
        string fullPath = $"{basePath}{spellId}";

        // Attempt to load the sprite
        Sprite spellIcon = Resources.Load<Sprite>(fullPath);

        // If the sprite is found, return it
        if (spellIcon != null)
        {
            return spellIcon;
        }

        // If no sprite is found, log a warning
        Debug.LogWarning($"Spell icon not found for spellId: {spellId}");
        return null;
    }

}
