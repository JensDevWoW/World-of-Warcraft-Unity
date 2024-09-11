using System.Collections.Generic;
using UnityEngine;
using Mirror; // Make sure Mirror is included

public class SpawnManager : MonoBehaviour
{
    public List<GameObject> prefabList; // List of prefabs to instantiate

    void Start()
    {
        SpawnObjects();
    }

    private void SpawnObjects()
    {
        List<SpawnData> spawnData = DatabaseManager.Instance.WorldData;

        foreach (var data in spawnData)
        {
            // Find the matching prefab by name
            GameObject prefab = prefabList.Find(p => p.name == data.PrefabName);

            if (prefab != null)
            {
                // Instantiate the prefab
                GameObject instance = Instantiate(prefab);

                // Set position if coordinates are provided
                if (data.HasCoordinates)
                {
                    instance.transform.position = new Vector3(data.X, data.Y, data.Z);
                }

                // Assign to the parent with the specified tag
                if (!string.IsNullOrEmpty(data.ParentTag))
                {
                    GameObject parent = GameObject.FindWithTag(data.ParentTag);
                    if (parent != null)
                    {
                        instance.transform.SetParent(parent.transform, false);
                    }
                }

                // Set anchored position if it's a UI element
                if (data.UI == 1)
                {
                    instance.GetComponent<RectTransform>().anchoredPosition = new Vector2(data.X, data.Y);
                }

                // Ensure the object has a NetworkIdentity and spawn it on the network
                NetworkIdentity networkIdentity = instance.GetComponent<NetworkIdentity>();
                if (networkIdentity != null)
                {
                    NetworkServer.Spawn(instance);
                }
                else
                {
                    Debug.LogError($"The prefab '{data.PrefabName}' does not have a NetworkIdentity component.");
                }
            }
        }
    }
}
