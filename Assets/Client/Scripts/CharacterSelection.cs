using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;
using TMPro;
public class CharacterSelectionManager : MonoBehaviour
{
    public GameObject charPrefab; // Prefab for character UI panel
    public Transform charListParent; // Parent panel where character UI panels will be added
    private Account currentAccount;
    private Character selectedCharacter; // Currently selected character
    public static CharacterSelectionManager Instance { get; private set; }
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

    public void LoadCharacters()
    {
        if (charListParent == null)
            charListParent = GameObject.Find("CharList").transform;
        // Get the logged-in account (assuming it's stored in a global state or via singleton)
        currentAccount = ClientAccountManager.Instance.GetCurrentAccount();

        if (currentAccount == null) return;

        // Retrieve all characters for the current account
        List<Character> characters = DatabaseManager.Instance.GetCharactersByAccountId(currentAccount.Id);

        // Create a UI panel for each character
        foreach (var character in characters)
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
            // Set the selected character as the login character (implement your logic to store this)
            Debug.Log($"Joining world as: {selectedCharacter.characterName}");

            // Disable all existing main cameras
            Camera[] cameras = Camera.allCameras;
            foreach (Camera cam in cameras)
            {
                cam.gameObject.SetActive(false);
            }

            //Send opcode
            NetworkWriter writer = new NetworkWriter();
            OpcodeMessage joinWorldPacket = new OpcodeMessage
            {
                opcode = Opcodes.CMSG_JOIN_WORLD,
                payload = writer.ToArray()
            };
            NetworkClient.Send(joinWorldPacket);


        }
        else
        {
            Debug.LogWarning("No character selected!");
        }
    }
}
