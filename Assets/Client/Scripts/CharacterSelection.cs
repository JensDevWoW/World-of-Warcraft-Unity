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

            // Use the captured character variable inside the lambda
            charPanel.GetComponent<Button>().onClick.AddListener(() => SelectCharacter(character, charPanel));
        }


        // Assign Join World button functionality
        GameObject joinWorldButton = GameObject.FindWithTag("JoinWorldButton");
        if (joinWorldButton != null)
        {
            joinWorldButton.GetComponent<Button>().onClick.AddListener(OnJoinButtonClicked);
        }
    }





    // Called when a character panel is clicked
    private void SelectCharacter(Character character, GameObject panel)
    {
        selectedCharacter = character;

        // Reset old char if we have it
        if (chosenChar != null)
            chosenChar.GetComponent<Image>().color = new Color32(56, 56, 56, 171);

        // Update new panel
        chosenChar = panel;

        panel.GetComponent<Image>().color = new Color32(189, 174, 85, 171);
       // SpawnCharTemplate(character);
        Debug.LogError($"Selected Character: {character.characterName}");
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

    // Called when the "Join" button is clicked
    private void OnJoinButtonClicked()
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
            writer.WriteInt(accountId);
            writer.WriteInt(selectedCharacter.characterId); // Send the character ID or necessary info

            OpcodeMessage joinWorldPacket = new OpcodeMessage
            {
                opcode = Opcodes.CMSG_JOIN_WORLD,
                payload = writer.ToArray()
            };
            NetworkClient.Send(joinWorldPacket);

            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }
        else
        {
            Debug.LogWarning("No character selected!");
        }
    }
}
