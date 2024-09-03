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

using System.Collections.Generic;
using UnityEngine;

public class BuffPosition : MonoBehaviour
{
    public GameObject template;
    public int columns = 5;
    public float spacing = 5f;
    private List<GameObject> activeBuffs = new List<GameObject>();

    void Start()
    {
        if (template == null)
        {
            template = transform.Find("Template").gameObject;
        }

        template.SetActive(false);
    }

    void Update()
    {
        PositionBuffs();
    }

    public void AddBuff(GameObject newBuff)
    {
        activeBuffs.Add(newBuff);
        PositionBuffs();
    }

    public void RemoveBuff(GameObject buff)
    {
        if (activeBuffs.Contains(buff))
        {
            activeBuffs.Remove(buff);
            PositionBuffs();
        }
    }

    private void PositionBuffs()
    {
        int index = 0;
        for (int i = 0; i < activeBuffs.Count; i++)
        {
            if (activeBuffs[i] == null)
            {
                activeBuffs.RemoveAt(i);
                i--;
                continue;
            }

            int row = index / columns;
            int col = index % columns;

            Vector3 newPosition = new Vector3(col * (template.GetComponent<RectTransform>().rect.width + spacing),
                                              -row * (template.GetComponent<RectTransform>().rect.height + spacing),
                                              0);

            activeBuffs[i].GetComponent<RectTransform>().anchoredPosition = newPosition;
            index++;
        }
    }
}
