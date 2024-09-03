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

public class ActionButton : MonoBehaviour
{
    public KeyCode keybind = KeyCode.Space; // Set your default keybind here
    public int spellId; // Add a spellId to link to the correct spell

    public Image buttonImage;
    public Image cooldownImage; // The background image to show the cooldown effect
    private Color originalColor = new Color32(255, 255, 255, 255);
    private Color darkColor = new Color32(119, 104, 104, 255); // The dark color to change to

    private float cooldownTime;
    private float cooldownTimer;
    private float gcdTime;
    private float gcdTimer;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(keybind))
        {
            DarkenButton();
        }
        else if (Input.GetKeyUp(keybind))
        {
            RestoreButton();
        }

        if (cooldownTimer > 0)
        {
            gcdTimer = 0;
            cooldownTimer -= Time.deltaTime;
            cooldownImage.fillAmount = Mathf.Clamp01(cooldownTimer / cooldownTime);
        }
        else if (gcdTimer > 0)
        {
            gcdTimer -= Time.deltaTime;
            cooldownImage.fillAmount = Mathf.Clamp01(gcdTimer / gcdTime);
        }
    }

    // Darken the button visually
    private void DarkenButton()
    {
        if (buttonImage != null)
        {
            buttonImage.color = darkColor;
        }
    }

    // Restore the button to its original color
    private void RestoreButton()
    {
        if (buttonImage != null)
        {
            buttonImage.color = originalColor;
        }
    }

    public void StartCooldown(float duration)
    {
        cooldownTime = duration;
        cooldownTimer = duration;
        cooldownImage.fillAmount = 1f;
    }

    public void StartGlobalCooldown(float duration)
    {
        gcdTime = duration;
        gcdTimer = duration;
        cooldownImage.fillAmount = 1f;
    }

    public bool IsOnCooldown()
    {
        return cooldownTimer > 0;
    }

    public bool IsOnGlobalCooldown()
    { return gcdTimer > 0; }
}
