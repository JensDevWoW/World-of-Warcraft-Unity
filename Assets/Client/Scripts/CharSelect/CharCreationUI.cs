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

    public void Build()
    {

        mage.onClick.AddListener(() => AssignClass(mage, 1));
        warlock.onClick.AddListener(() => AssignClass(warlock, 2));
        rogue.onClick.AddListener(() => AssignClass(rogue, 3));

        human.onClick.AddListener(() => AssignRace(human, 1));
        dwarf.onClick.AddListener(() => AssignRace(dwarf, 2));
        nightelf.onClick.AddListener(() => AssignRace(nightelf, 3));
        draenei.onClick.AddListener(() => AssignRace(draenei, 4));
        worgen.onClick.AddListener(() => AssignRace(worgen, 5));
        alliancepanda.onClick.AddListener(() => AssignRace(alliancepanda, 6));
        orc.onClick.AddListener(() => AssignRace(orc, 7));
        undead.onClick.AddListener(() => AssignRace(undead, 8));
        tauren.onClick.AddListener(() => AssignRace(tauren, 9));
        troll.onClick.AddListener(() => AssignRace(troll, 10));
        bloodelf.onClick.AddListener(() => AssignRace(bloodelf, 11));
        goblin.onClick.AddListener(() => AssignRace(goblin, 12));
        hordepanda.onClick.AddListener(() => AssignRace(hordepanda, 13));

        customize.onClick.AddListener(() => Customize());

        AssignRace(bloodelf, 11);
        AssignClass(mage, 1);
        selectedBodyType = 2; // Female
        LoadCharacterIntoFrame();
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
            gameObject.SetActive(false);
            customizeCanvas.SetActive(true);
            return;
        }
    }
}
