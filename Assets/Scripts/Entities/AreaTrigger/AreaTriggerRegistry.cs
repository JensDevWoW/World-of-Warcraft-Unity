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
