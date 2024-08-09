using System.IO;
using UnityEngine;

public class KeyBindManager : MonoBehaviour
{
    public static KeyBinds keyBinds;
    private static string keyBindsFilePath;

    private void Awake()
    {
        keyBindsFilePath = Path.Combine(Application.persistentDataPath, "keybinds.json");
        LoadKeyBinds();
    }

    public static void SaveKeyBinds()
    {
        string json = JsonUtility.ToJson(keyBinds, true);
        File.WriteAllText(keyBindsFilePath, json);
    }

    public static void LoadKeyBinds()
    {
        if (File.Exists(keyBindsFilePath))
        {
            string json = File.ReadAllText(keyBindsFilePath);
            keyBinds = JsonUtility.FromJson<KeyBinds>(json);
        }
        else
        {
            keyBinds = new KeyBinds(); // Load default keybinds
            SaveKeyBinds(); // Save the default keybinds to the file
        }
    }

    private void OnApplicationQuit()
    {
        SaveKeyBinds(); // Save the keybinds when the game closes
    }
}
