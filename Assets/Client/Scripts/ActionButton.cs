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


using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    public KeyCode keybind = KeyCode.Space; // Set your default keybind here
    public int spellId = 0; // Add a spellId to link to the correct spell

    public Image buttonImage;
    public Image cooldownImage; // The background image to show the cooldown effect
    public GameObject Charges;
    private TextMeshProUGUI ChargeText;
    private Color originalColor = new Color32(255, 255, 255, 255);
    private Color darkColor = new Color32(119, 104, 104, 255); // The dark color to change to

    private float cooldownTime;
    private float cooldownTimer;
    private float gcdTime;
    private float gcdTimer;
    private int m_charges;
    private bool m_offGCD = false;
    SpellInfo currentSpell;
    public void Init()
    {
        if (spellId != 0)
        {
            this.buttonImage.sprite = IconManager.GetSpellIcon(spellId);
            this.cooldownImage.sprite = IconManager.GetSpellIcon(spellId);
            this.buttonImage.color = originalColor;
            this.currentSpell = SpellContainer.Instance.GetSpellById(spellId);
           
            ChargeText = Charges.GetComponent<TextMeshProUGUI>();

            if (currentSpell != null)
            {
                this.m_offGCD = currentSpell.HasFlag(SpellFlags.SPELL_FLAG_IGNORES_GCD);
                if (currentSpell.Stacks > 1)
                {
                    if (ChargeText != null)
                    {
                        ChargeText.text = currentSpell.Stacks.ToString();
                        ChargeText.gameObject.SetActive(true);
                    }
                }
            }
        }
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

    // TODO: Add in functionality to get local client spell cooldowns rather than from SpellData
    public void UpdateCharges(int charges)
    {
        ChargeText.text = charges.ToString();
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

    public void StartLightCooldown(float duration)
    {
        cooldownTime = duration;
        cooldownTimer = duration;
        cooldownImage.color = originalColor;
        cooldownImage.fillAmount = 1f;
    }

    public void StartCooldown(float duration)
    {
        cooldownTime = duration;
        cooldownTimer = duration;
        cooldownImage.color = darkColor;
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

    public bool OffGCD()
    {
        return m_offGCD;
    }    
    public bool IsOnGlobalCooldown()
    { return gcdTimer > 0; }

    public void UpdateButton(int spellId, Sprite icon)
    {
        this.spellId = spellId;
        this.buttonImage.sprite = icon;
        this.cooldownImage.sprite = icon;
        this.buttonImage.color = originalColor;
        this.m_offGCD = SpellContainer.Instance.GetSpellById(spellId).HasFlag(SpellFlags.SPELL_FLAG_CAST_WHILE_CASTING) ? true : false;
    }
}
