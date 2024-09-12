using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.SceneManagement;

public class Character_Load : MonoBehaviour
{
    // Start is called before the first frame update
    private bool _loaded = false;
    private float _loadTime = 2;
    private bool _loadingStarted = false;
    private void Update()
    {
        if (ClientAccountManager.Instance.GetCurrentAccount() != null && _loaded == false)
        {
            if (SceneManager.GetSceneByName("CharacterSelectionScene").isLoaded == true)
            {
                CharacterSelectionManager.Instance.LoadCharacters();
                _loaded = true;
                Destroy(this);
            }
        }
    }
}
