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

public class SpellScriptRegistry
{
    // A dictionary to map spell IDs to SpellScript types
    private static readonly Dictionary<int, Type> spellScriptMap = new Dictionary<int, Type>
    {
        { 13, typeof(PyroblastScript) },
        { 15, typeof(IceLance) },
        { 2, typeof(Frostbolt) },
        // Add more mappings as needed
    };

    public static Type GetSpellScriptType(int spellId)
    {
        if (spellScriptMap.TryGetValue(spellId, out Type scriptType))
        {
            return scriptType;
        }
        return null;
    }
}
