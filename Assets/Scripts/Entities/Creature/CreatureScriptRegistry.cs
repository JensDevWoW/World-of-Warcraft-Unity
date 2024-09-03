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

public class CreatureScriptRegistry
{
    // A dictionary to map spell IDs to SpellScript types
    private static readonly Dictionary<int, Type> creatureScriptMap = new Dictionary<int, Type>
    {
        { 1, typeof(TrainingDummy) }, // Here is where all creaturescripts are linked to their respective creaturescripts
        // Add more mappings as needed
    };

    public static Type GetCreatureScriptType(int creatureId)
    {
        if (creatureScriptMap.TryGetValue(creatureId, out Type scriptType))
        {
            return scriptType;
        }
        return null;
    }
}
