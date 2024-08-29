using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[System.Flags]
public enum AreaTriggerAttributes
{
    AREA_TRIGGER_ATTR_NONE = 0,
    AREA_TRIGGER_ATTR_PERSISTENT = 1 << 0,
    AREA_TRIGGER_ATTR_VISIBLE = 1 << 1,
    // Add more attributes as needed
}

[System.Serializable]
[System.Flags]
public enum AreaTriggerFlags
{
    AREA_TRIGGER_FLAG_NONE = 0,
    AREA_TRIGGER_FLAG_TRIGGERS_ON_ENTER = 1 << 0,
    AREA_TRIGGER_FLAG_TRIGGERS_ON_LEAVE = 1 << 1,
    // Add more flags as needed
}

[System.Serializable]
public class AreaTriggerData
{
    public int id;
    public string name;
    public float distance;
    public float duration;
    public string attributes;  // Serialized AreaTriggerAttributes
    public string flags;       // Serialized AreaTriggerFlags
}

[System.Serializable]
public class AreaTriggerDataWrapper
{
    public AreaTriggerData[] areaTriggers;
}

public class AreaTriggerDataHandler : MonoBehaviour
{
    public static AreaTriggerDataHandler Instance { get; private set; }
    public List<AreaTriggerInfo> AreaTriggers { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadAreaTriggerData();
    }

    void LoadAreaTriggerData()
    {
        TextAsset jsonData = Resources.Load<TextAsset>("AreaTriggerData");

        if (jsonData == null)
        {
            Debug.LogError("Failed to load AreaTriggerData.json from Resources.");
            return;
        }

        Debug.Log("Loaded JSON: " + jsonData.text);  // Print JSON content

        AreaTriggerDataWrapper areaTriggerDataWrapper = JsonUtility.FromJson<AreaTriggerDataWrapper>(jsonData.text);

        if (areaTriggerDataWrapper == null || areaTriggerDataWrapper.areaTriggers == null || areaTriggerDataWrapper.areaTriggers.Length == 0)
        {
            Debug.LogError("Failed to parse AreaTriggerData or AreaTriggerData array is empty.");
            return;
        }

        AreaTriggers = new List<AreaTriggerInfo>();

        foreach (var areaTriggerData in areaTriggerDataWrapper.areaTriggers)
        {
            AreaTriggerAttributes attributes = ParseAreaTriggerAttributes(areaTriggerData.attributes);
            AreaTriggerFlags flags = ParseAreaTriggerFlags(areaTriggerData.flags);

            AreaTriggerInfo areaTrigger = new AreaTriggerInfo(
                areaTriggerData.id,
                areaTriggerData.name,
                areaTriggerData.distance,
                areaTriggerData.duration,
                attributes,
                flags
            );

            AreaTriggers.Add(areaTrigger);
        }

        Debug.Log("Successfully loaded and parsed AreaTriggers.");
    }

    AreaTriggerAttributes ParseAreaTriggerAttributes(string attributesString)
    {
        AreaTriggerAttributes attributes = AreaTriggerAttributes.AREA_TRIGGER_ATTR_NONE;

        if (!string.IsNullOrEmpty(attributesString))
        {
            string[] attributeNames = attributesString.Split(',');

            foreach (var attribute in attributeNames)
            {
                string trimmedAttributeName = attribute.Trim();  // Use a new variable for the trimmed value

                if (Enum.TryParse(trimmedAttributeName, out AreaTriggerAttributes parsedAttribute))
                {
                    attributes |= parsedAttribute;
                }
                else
                {
                    Debug.LogWarning($"Unknown AreaTriggerAttribute: {trimmedAttributeName}");
                }
            }
        }

        return attributes;
    }

    AreaTriggerFlags ParseAreaTriggerFlags(string flagsString)
    {
        AreaTriggerFlags flags = AreaTriggerFlags.AREA_TRIGGER_FLAG_NONE;

        if (!string.IsNullOrEmpty(flagsString))
        {
            string[] flagNames = flagsString.Split(',');

            foreach (var flag in flagNames)
            {
                string trimmedFlagName = flag.Trim();  // Use a new variable for the trimmed value

                if (Enum.TryParse(trimmedFlagName, out AreaTriggerFlags parsedFlag))
                {
                    flags |= parsedFlag;
                }
                else
                {
                    Debug.LogWarning($"Unknown AreaTriggerFlag: {trimmedFlagName}");
                }
            }
        }

        return flags;
    }
}

public class AreaTriggerInfo
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public float Distance { get; private set; }
    public float Duration { get; private set; }
    public AreaTriggerAttributes Attributes { get; private set; }
    public AreaTriggerFlags Flags { get; private set; }

    public AreaTriggerInfo(int id, string name, float distance, float duration, AreaTriggerAttributes attributes, AreaTriggerFlags flags)
    {
        Id = id;
        Name = name;
        Distance = distance;
        Duration = duration;
        Attributes = attributes;
        Flags = flags;
    }

    // Additional methods for AreaTrigger behavior can be added here
}
