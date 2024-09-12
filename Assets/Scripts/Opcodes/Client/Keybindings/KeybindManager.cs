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

using System.IO;
using UnityEngine;

public class KeyBindManager : MonoBehaviour
{
    public static KeyBinds keyBinds;
    private static string keyBindsFilePath;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

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
