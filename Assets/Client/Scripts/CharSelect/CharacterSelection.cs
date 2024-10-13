using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;
using TMPro;
using UnityEngine.TextCore.Text;

public class CharacterSelectionManager : MonoBehaviour
{
    public GameObject charPrefab; // Prefab for character UI panel
    public Transform charListParent; // Parent panel where character UI panels will be added
    private Character selectedCharacter; // Currently selected character
    public GameObject charTemplatePrefab; // Prefab for the character template
    public GameObject charNameObj;
    public TextMeshProUGUI charName;
    private GameObject loadedChar;
    public bool LoadingComplete;
    public RuntimeAnimatorController BElfAnimationController;
    private Dictionary<Character, GameObject> characterPanels = new Dictionary<Character, GameObject>();
    public static CharacterSelectionManager Instance { get; private set; }
    public List<Character> characterList = new List<Character>(); // List to store characters from the server
    public GameObject chosenChar = null;
    private void Awake()
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

    private void Update()
    {
        
    }

    // Call this function when the client receives character data from the server
    public void ReceiveCharacterList(List<Character> characters)
    {
        characterList = characters;
    }
    public bool HasReceivedCharacterList()
    {
        return characterList != null && characterList.Count > 0;
    }
    // Load the character UI panels from the received character list
    public void LoadCharacters()
    {
        if (charListParent == null)
            charListParent = GameObject.Find("CharList").transform;

        // Clear existing UI panels
        foreach (Transform child in charListParent)
        {
            Destroy(child.gameObject);
        }

        characterPanels.Clear();

        // Create a UI panel for each character
        float panelHeight = 0.15f; // Relative height of each panel
        float topOffset = 0.01f; // Offset from the top
        int totalCharacters = characterList.Count;

        for (int i = 0; i < totalCharacters; i++)
        {
            GameObject charPanel = Instantiate(charPrefab, charListParent);

            // Store the character in a local variable
            var character = characterList[i];

            // Set the panel's text
            charPanel.GetComponentInChildren<TextMeshProUGUI>().text = character.characterName;

            // Adjust the RectTransform anchor and position
            RectTransform rt = charPanel.GetComponent<RectTransform>();

            rt.anchorMin = new Vector2(0.05f, 1f - (topOffset + panelHeight * (i + 1)));
            rt.anchorMax = new Vector2(0.95f, 1f - (topOffset + panelHeight * i));

            rt.offsetMin = new Vector2(0, 0); // Remove offsets
            rt.offsetMax = new Vector2(0, 0);

            characterPanels.Add(character, charPanel);

            // Use the captured character variable inside the lambda
            charPanel.GetComponent<Button>().onClick.AddListener(() => SelectCharacter(character, charPanel));
        }


        // Assign Join World button functionality
        GameObject joinWorldButton = GameObject.FindWithTag("JoinWorldButton");
        if (joinWorldButton != null)
        {
            joinWorldButton.GetComponent<Button>().onClick.AddListener(OnJoinButtonClicked);
        }

        GameObject backButton = GameObject.FindWithTag("BackButtonCharSelect");
        if (backButton != null)
        {
            backButton.GetComponent<Button>().onClick.AddListener(Back);
        }

        // Load CharName obj
        GameObject charName1 = GameObject.FindWithTag("CharName");
        if (charName1 != null)
        {
            charName = charName1.GetComponent<TextMeshProUGUI>();
        }

        // When WoW starts, first listed char is always chosen (actually it's last played char but we will get there eventually)
        // Choose first char in list:
        // Select the first character in the list with its panel
        if (characterList.Count > 0)
        {
            var firstCharacter = characterList[0];
            SelectCharacter(firstCharacter, characterPanels[firstCharacter]);
        }
    }

    private void Back()
    {
        if (NetworkClient.isConnected)
        {
            // Stop the client connection and disconnect
            NetworkManager.singleton.StopClient();
            Debug.Log("Client disconnected. Returning to login screen.");

            // Load the login scene
            SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
        }
        else
        {
            Debug.LogWarning("Client is not connected, unable to disconnect.");
        }
    }


    private void Highlight(Image image)
    {
        if (image != null)
        {
            image.color = new Color32(189, 174, 85, 171);
        }
    }

    private void Unhighlight(Image image)
    {
        if (image != null)
        {
            image.color = new Color32(56, 56, 56, 171);
        }
    }

    private void LoadCharacterIntoFrame(Character character)
    {
        if (loadedChar)
            Destroy(loadedChar);

        Vector3 position = new Vector3(0.124f, 4.979f, -5.85f);
        Quaternion rotation = Quaternion.Euler(0, -90, 0);
        Vector3 scale = new Vector3(1, 1, 1);
        GameObject charModel = DatabaseManager.Instance.LoadCharacterModel(character);

        if (charModel != null)
        {
            GameObject model = Instantiate(charModel, position, rotation);
            model.transform.localScale = scale;
            loadedChar = model;
        }
        else
            Debug.LogError("No model found!");
    }

    public void DeleteLoadedChar()
    {
        if (loadedChar != null)
            Destroy(loadedChar);
    }

    // Called when a character panel is clicked
    private void SelectCharacter(Character character, GameObject panel)
    {
        selectedCharacter = character;
        LoadCharacterIntoFrame(selectedCharacter);
        charName.text = character.characterName;

        // Reset old char if we have it
        if (chosenChar != null)
            Unhighlight(chosenChar.GetComponent<Image>());

        // Update new panel
        chosenChar = panel;

        Highlight(panel.GetComponent<Image>());
    }

    private void SpawnCharTemplate(Character character)
    {
        if (charTemplatePrefab == null)
            return;

        Vector3 spawnPosition = new Vector3(1.59f, 0.75f, -1.03f);
        Quaternion rotation = Quaternion.Euler(0, -89.347f, 0); // Rotate around Y-axis
        GameObject prefab = Instantiate(charPrefab, spawnPosition, rotation);

        if (prefab != null) 
        {
            ClientDebug.Instance.Log("It's here! Just can't see it, dumbass.");
        }

    }

    public void LoadingFinished()
    {
        JoinGame();
    }

    private void JoinGame()
    {
        if (selectedCharacter != null)
        {
            Debug.Log($"Joining world as: {selectedCharacter.characterName}");

            // Disable all existing main cameras
            Camera[] cameras = Camera.allCameras;
            foreach (Camera cam in cameras)
            {
                cam.gameObject.SetActive(false);
            }

            int accountId = ClientAccountManager.Instance.GetCurrentAccount().Id;

            // Send the opcode to join the world
            NetworkWriter writer = new NetworkWriter();
            //writer.WriteInt(accountId);
            writer.WriteInt(selectedCharacter.characterId); // Send the character ID or necessary info

            OpcodeMessage joinWorldPacket = new OpcodeMessage
            {
                opcode = Opcodes.CMSG_JOIN_WORLD,
                payload = writer.ToArray()
            };
            NetworkClient.Send(joinWorldPacket);

            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            //SceneManager.LoadScene("LoadingScreen", LoadSceneMode.Single);
        }
        else
        {
            Debug.LogWarning("No character selected!");
        }
    }

    // Called when the "Join" button is clicked
    private void OnJoinButtonClicked()
    {
        if (selectedCharacter != null)
        {
            // SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            SceneManager.LoadScene("LoadingScreen", LoadSceneMode.Single);
        }
        else
        {
            Debug.LogWarning("No character selected!");
        }
    }
}
