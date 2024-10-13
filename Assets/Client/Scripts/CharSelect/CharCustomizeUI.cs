using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using UnityEngine.SceneManagement;

public class CharCustomize : MonoBehaviour
{
    // Start is called before the first frame update
    public static CharCustomize Instance { get; private set; }

    public GameObject currentCharacter;

    // Start of Customize
    private Vector3 startPos = new Vector3(0.139f, 6.085f, -8.02f);
    private Vector3 startRot = new Vector3(2.855f, 0, 0);

    private Vector3 endPos = new Vector3(0.139f, 6.75f, -6.511f);
    private Vector3 endRot = new Vector3(0, 0, 0);

    public Button BackButton;
    public Button FinishButton;
    public Button RandomName;
    public TMP_InputField charName;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  // Ensure there's only one instance of this script
        }
    }

    private void ShowCanvas(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1; // Make it visible
        canvasGroup.interactable = true; // Allow interaction
        canvasGroup.blocksRaycasts = true; // Block UI raycasts for clicks
    }

    // Function to hide a canvas
    private void HideCanvas(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0; // Make it invisible
        canvasGroup.interactable = false; // Disable interaction
        canvasGroup.blocksRaycasts = false; // Allow other UI elements to be clicked
    }

    public void Build()
    {
        BackButton.onClick.RemoveAllListeners();
        FinishButton.onClick.RemoveAllListeners();

        BackButton.onClick.AddListener(Back);
        FinishButton.onClick.AddListener(Finish);
        currentCharacter.GetComponent<Animator>().SetTrigger("Stand");
        LinIntCam.Instance.StartLinearInterpolation(startPos, endPos, startRot, endRot, 2);
    }



    public void Back()
    {
        LinIntCam.Instance.StartLinearInterpolation(endPos, startPos, endRot, startRot, 2);
        currentCharacter.GetComponent<Animator>().SetTrigger("Back");
        GameObject canvas = GameObject.FindWithTag("CharacterCreationCanvas");
        HideCanvas(gameObject.GetComponent<CanvasGroup>());
        ShowCanvas(canvas.GetComponent<CanvasGroup>());
        CharCreationUI.Instance.Build(false, false);
    }

    public void Finish()
    {
        string name = charName.text;
        int classId = CharCreationUI.Instance.selectedClassId;
        int raceId = CharCreationUI.Instance.selectedRaceId;
        int factionId = 2;
        int bodyType = CharCreationUI.Instance.selectedBodyType;

        NetworkWriter writer = new NetworkWriter();

        writer.WriteString(name);
        writer.WriteInt(classId);
        writer.WriteInt(raceId);
        writer.WriteInt(factionId);
        writer.WriteInt(bodyType);
        //TODO: Add more data to send

        OpcodeMessage msg = new OpcodeMessage
        {
            opcode = Opcodes.CMSG_CREATE_CHAR,
            payload = writer.ToArray()
        };

        NetworkClient.Send(msg);

        Debug.Log("New character created and added to database.");
        SceneManager.LoadScene("CharacterSelectionScene");
        Character_Load.Instance.loaded = false;
    }

}
