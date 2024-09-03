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

[System.Serializable]
public class ControlBinding
{
    public KeyCode[] primary = new KeyCode[1], secondary;

    public bool GetControlBinding()
    {
        bool primaryPressed = false, secondaryPressed = false;

        //Primary
        if(primary.Length == 1)
        {
            if (Input.GetKey(primary[0]))
                primaryPressed = true;
        }
        else if(primary.Length == 2)
        {
            if (Input.GetKey(primary[0]) && Input.GetKey(primary[1]))
                primaryPressed = true;
        }

        //Secondary
        if (secondary.Length == 1)
        {
            if (Input.GetKey(secondary[0]))
                secondaryPressed = true;
        }
        else if (secondary.Length == 2)
        {
            if (Input.GetKey(secondary[0]) && Input.GetKey(secondary[1]))
                secondaryPressed = true;
        }

        //Check Keybindings
        if (primaryPressed || secondaryPressed)
            return true;

        return false;
    }

    bool unpressed = false;

    public bool GetControlBindingDown()
    {
        bool primaryPressed = false, secondaryPressed = false;

        //Primary
        if (primary.Length == 1)
        {
            if (Input.GetKey(primary[0]))
                primaryPressed = true;
        }
        else if (primary.Length == 2)
        {
            if (Input.GetKey(primary[0]) && Input.GetKey(primary[1]))
                primaryPressed = true;
        }

        //Secondary
        if (secondary.Length == 1)
        {
            if (Input.GetKey(secondary[0]))
                secondaryPressed = true;
        }
        else if (secondary.Length == 2)
        {
            if (Input.GetKey(secondary[0]) && Input.GetKey(secondary[1]))
                secondaryPressed = true;
        }

        if(!unpressed)
        {
            if(primaryPressed || secondaryPressed)
            {
                unpressed = true;
                return true;
            }
        }
        else
        {
            if(!primaryPressed && !secondaryPressed)
                unpressed = false;
        }

        return false;
    }
}
