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

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KeyBinds
{
    public KeyCode one;
    public KeyCode two;
    public KeyCode three;
    public KeyCode four;
    public KeyCode five;
    public KeyCode six;
    public KeyCode seven;
    public KeyCode eight;
    public KeyCode nine;
    public KeyCode zero;

    public KeyBinds(KeyCode one, KeyCode two, KeyCode three, KeyCode four, KeyCode five, KeyCode six, KeyCode seven, KeyCode eight, KeyCode nine, KeyCode zero)
    {
        this.one = one; this.two = two; this.three = three; this.four = four; this.five = five; this.six = six; this.seven = seven;
        this.eight = eight; this.nine = nine; this.zero = zero;
    }

    public KeyBinds()
    {
        ResetToDefault();
    }

    public void ResetToDefault()
    {
        one = KeyCode.Alpha1;
        two = KeyCode.Alpha2;
        three = KeyCode.Alpha3;
        four = KeyCode.Alpha4;
        five = KeyCode.Alpha5;
        six = KeyCode.Alpha6;
        seven = KeyCode.Alpha7;
        eight = KeyCode.Alpha8;
        nine = KeyCode.Alpha9;
        zero = KeyCode.Alpha0;
    }
}
