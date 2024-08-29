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
}
