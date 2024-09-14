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
    public static CharacterSelectionManager Instance { get; private set; }
    public List<Character> characterList = new List<Character>(); // List to store characters from the server

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
        foreach (var character in characterList)
        {
            GameObject charPanel = Instantiate(charPrefab, charListParent);
            charPanel.GetComponentInChildren<TextMeshProUGUI>().text = character.characterName; // Assuming the prefab has a Text component
            charPanel.GetComponent<Button>().onClick.AddListener(() => SelectCharacter(character));
        }

        GameObject joinWorldButton = GameObject.FindWithTag("JoinWorldButton");
        if (joinWorldButton != null)
        {
            joinWorldButton.GetComponent<Button>().onClick.AddListener(OnJoinButtonClicked);
        }
    }

    // Called when a character panel is clicked
    private void SelectCharacter(Character character)
    {
        selectedCharacter = character;
        Debug.Log($"Selected Character: {character.characterName}");
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
            writer.WriteInt(accountId); // TODO: Get AccountId
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
