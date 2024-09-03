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
using UnityEngine.UI;

public class TargetHealthBar : MonoBehaviour
{
    public Image bar;  // Reference to the Image component representing the health bar

    public void Start()
    {
        UpdateHealth(1000, 1000);
    }
    // Method to update the health bar with the new health and max health values
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        // Ensure the health values are within valid ranges
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Calculate the fill amount based on the current health
        float fillAmount = currentHealth / maxHealth;

        // Update the UI element
        bar.fillAmount = Mathf.Clamp01(fillAmount);
    }
}
