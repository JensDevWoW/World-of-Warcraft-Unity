using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharCreationUI : MonoBehaviour
{

    public Button customize;

    // [[ Class ]] \\
    public Button mage;
    public Button warlock;
    public Button rogue;

    // [[ Race ]] \\
    public Button human;
    public Button dwarf;
    public Button nightelf;
    public Button gnome;
    public Button draenei;
    public Button worgen;
    public Button alliancepanda;
    public Button orc;
    public Button undead;
    public Button tauren;
    public Button troll;
    public Button bloodelf;
    public Button goblin;
    public Button hordepanda;

    public Button selectedClass;
    public Button selectedRace;

    public GameObject loadedChar;

    public GameObject orcMale;
    public GameObject orcFemale;

    public GameObject belfMale;
    public GameObject belfFemale;

    public static CharCreationUI Instance { get; private set; }

    public int selectedClassId = 0;
    public int selectedRaceId = 0;
    public int selectedBodyType = 0;

    // Listener references to remove later
    private UnityEngine.Events.UnityAction assignClassMage;
    private UnityEngine.Events.UnityAction assignClassWarlock;
    private UnityEngine.Events.UnityAction assignClassRogue;

    private UnityEngine.Events.UnityAction assignRaceHuman;
    private UnityEngine.Events.UnityAction assignRaceDwarf;
    private UnityEngine.Events.UnityAction assignRaceNightelf;
    private UnityEngine.Events.UnityAction assignRaceDraenei;
    private UnityEngine.Events.UnityAction assignRaceWorgen;
    private UnityEngine.Events.UnityAction assignRaceAlliancepanda;
    private UnityEngine.Events.UnityAction assignRaceOrc;
    private UnityEngine.Events.UnityAction assignRaceUndead;
    private UnityEngine.Events.UnityAction assignRaceTauren;
    private UnityEngine.Events.UnityAction assignRaceTroll;
    private UnityEngine.Events.UnityAction assignRaceBloodelf;
    private UnityEngine.Events.UnityAction assignRaceGoblin;
    private UnityEngine.Events.UnityAction assignRaceHordepanda;

    private UnityEngine.Events.UnityAction customizeAction;

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

    public void Build(bool cam, bool loadChar)
    {
        // Assign each listener to a variable for future removal
        assignClassMage = () => AssignClass(mage, 1);
        assignClassWarlock = () => AssignClass(warlock, 2);
        assignClassRogue = () => AssignClass(rogue, 3);

        assignRaceHuman = () => AssignRace(human, 1);
        assignRaceDwarf = () => AssignRace(dwarf, 2);
        assignRaceNightelf = () => AssignRace(nightelf, 3);
        assignRaceDraenei = () => AssignRace(draenei, 4);
        assignRaceWorgen = () => AssignRace(worgen, 5);
        assignRaceAlliancepanda = () => AssignRace(alliancepanda, 6);
        assignRaceOrc = () => AssignRace(orc, 7);
        assignRaceUndead = () => AssignRace(undead, 8);
        assignRaceTauren = () => AssignRace(tauren, 9);
        assignRaceTroll = () => AssignRace(troll, 10);
        assignRaceBloodelf = () => AssignRace(bloodelf, 11);
        assignRaceGoblin = () => AssignRace(goblin, 12);
        assignRaceHordepanda = () => AssignRace(hordepanda, 13);

        customizeAction = () => Customize();

        // Add listeners
        mage.onClick.AddListener(assignClassMage);
        warlock.onClick.AddListener(assignClassWarlock);
        rogue.onClick.AddListener(assignClassRogue);

        human.onClick.AddListener(assignRaceHuman);
        dwarf.onClick.AddListener(assignRaceDwarf);
        nightelf.onClick.AddListener(assignRaceNightelf);
        draenei.onClick.AddListener(assignRaceDraenei);
        worgen.onClick.AddListener(assignRaceWorgen);
        alliancepanda.onClick.AddListener(assignRaceAlliancepanda);
        orc.onClick.AddListener(assignRaceOrc);
        undead.onClick.AddListener(assignRaceUndead);
        tauren.onClick.AddListener(assignRaceTauren);
        troll.onClick.AddListener(assignRaceTroll);
        bloodelf.onClick.AddListener(assignRaceBloodelf);
        goblin.onClick.AddListener(assignRaceGoblin);
        hordepanda.onClick.AddListener(assignRaceHordepanda);

        customize.onClick.AddListener(customizeAction);

        AssignRace(bloodelf, 11);
        AssignClass(mage, 1);
        selectedBodyType = 2; // Female

        // Checks
        if (loadChar)
            LoadCharacterIntoFrame();
        if (cam)
            CameraBezierMovement.Instance.StartCameraMovement();
    }

    public void RemoveListeners()
    {
        // Remove listeners
        mage.onClick.RemoveListener(assignClassMage);
        warlock.onClick.RemoveListener(assignClassWarlock);
        rogue.onClick.RemoveListener(assignClassRogue);

        human.onClick.RemoveListener(assignRaceHuman);
        dwarf.onClick.RemoveListener(assignRaceDwarf);
        nightelf.onClick.RemoveListener(assignRaceNightelf);
        draenei.onClick.RemoveListener(assignRaceDraenei);
        worgen.onClick.RemoveListener(assignRaceWorgen);
        alliancepanda.onClick.RemoveListener(assignRaceAlliancepanda);
        orc.onClick.RemoveListener(assignRaceOrc);
        undead.onClick.RemoveListener(assignRaceUndead);
        tauren.onClick.RemoveListener(assignRaceTauren);
        troll.onClick.RemoveListener(assignRaceTroll);
        bloodelf.onClick.RemoveListener(assignRaceBloodelf);
        goblin.onClick.RemoveListener(assignRaceGoblin);
        hordepanda.onClick.RemoveListener(assignRaceHordepanda);

        customize.onClick.RemoveListener(customizeAction);
    }

    private void Back()
    {
        RemoveListeners();
        // TODO: Add in a way to reload the char selection screen from here
    }

    private GameObject LoadModel(int raceId, int classId, int bodyType)
    {
        switch (raceId)
        {
            case 7: // Orc
                if (classId == 1)
                    if (bodyType == 1)
                        return orcMale;
                    else
                        return orcFemale;
                break;
            case 11: // Blood elf
                if (classId == 1)
                    if (bodyType == 1)
                        return belfMale;
                    else
                        return belfFemale;
                break;
        }

        return null;
    }

    private void LoadCharacterIntoFrame()
    {
        if (loadedChar)
            Destroy(loadedChar);

        Vector3 position = new Vector3(0.124f, 4.979f, -5.85f);
        Quaternion rotation = Quaternion.Euler(0, -90, 0);
        Vector3 scale = new Vector3(1, 1, 1);

        GameObject charModel = LoadModel(selectedRaceId, selectedClassId, selectedBodyType);

        if (charModel != null)
        {
            GameObject model = Instantiate(charModel, position, rotation);
            model.transform.localScale = scale;
            loadedChar = model;
        }
    }

    void HighlightClass(Button button)
    {
        button.GetComponent<Image>().color = new Color32(255, 255, 0, 255);
        selectedClass = button;
    }

    void HighlightRace(Button button)
    {
        selectedRace = button;
    }

    void Unhighlight(Button button)
    {
        button.GetComponent<Image>().color = new Color32(255, 255, 255, 125);
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
    void AssignClass(Button button, int classId)
    {
        selectedClassId = classId;
        LoadCharacterIntoFrame();
        Debug.Log("Selected Class ID: " + selectedClassId);
        if (selectedClass != null)
            Unhighlight(selectedClass);

        HighlightClass(button);
    }

    void AssignRace(Button button, int raceId)
    {
        selectedRaceId = raceId;
        LoadCharacterIntoFrame();
        Debug.Log("Selected Class ID: " + selectedRaceId);
        if (selectedRace != null)
            Unhighlight(selectedRace);

        HighlightRace(button);
    }

    void Customize()
    {
        GameObject customizeCanvas = GameObject.FindWithTag("CharacterCustomizeCanvas");
        if (customizeCanvas != null)
        {
            HideCanvas(gameObject.GetComponent<CanvasGroup>());
            ShowCanvas(customizeCanvas.GetComponent<CanvasGroup>());
            CharCustomize.Instance.currentCharacter = loadedChar;
            CharCustomize.Instance.Build();
            return;
        }
    }
}
