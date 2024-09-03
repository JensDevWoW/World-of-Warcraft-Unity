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
using TMPro;
using Mirror;

public class BuffDebuff : MonoBehaviour
{
    public int spellId;
    public Image buffIcon;
    public float timer;
    public TMP_Text textBox;

    private float currentTime;

    private void Start()
    {
        currentTime = timer;
    }

    private void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
                Destroy(gameObject);
            UpdateTextBox();
        }
    }

    private void UpdateTextBox()
    {
        if (currentTime < 60)
            textBox.text = Mathf.Ceil(currentTime).ToString() + "s";
        else if (currentTime < 3600)
            textBox.text = Mathf.Floor(currentTime / 60).ToString() + "m";
        else
            textBox.text = Mathf.Floor(currentTime / 3600).ToString() + "h";
    }

    public void UpdateData(float duration, int stacks)
    {
        if (duration == 0)
            Destroy(gameObject);
        else
        {
            this.timer = duration;
            currentTime = duration;
            UpdateTextBox();
        }
    }

    public void InitializeBuff(int spellId, Sprite icon, float duration)
    {
        this.spellId = spellId;
        this.buffIcon.sprite = icon;
        this.timer = duration;
        currentTime = duration;
        UpdateTextBox();
    }
}
