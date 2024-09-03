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

public class AreaTriggerScriptRegistry
{
    // A dictionary to map area trigger IDs to AreaTriggerScript types
    private static readonly Dictionary<int, Type> areaTriggerScriptMap = new Dictionary<int, Type>
    {
        //{ 101, typeof(FrostRingScript) },
        // Add more mappings as needed
    };

    public static Type GetAreaTriggerScriptType(int areaTriggerId)
    {
        if (areaTriggerScriptMap.TryGetValue(areaTriggerId, out Type scriptType))
        {
            return scriptType;
        }
        return null;
    }
}
