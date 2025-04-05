using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ModelHandler : MonoBehaviour
{
    public static ModelHandler Instance { get; private set; }


    // Character Models \\

    // [[ Human ]] \\ 1
    public GameObject humanFemaleModel;
    public GameObject humanMaleModel;

    // [[ Dwarf ]] \\ 2
    public GameObject dwarfMaleModel;
    public GameObject dwarfFemaleModel;

    // [[ Night Elf ]] \\ 3
    public GameObject nelfFemaleModel;
    public GameObject nelfMaleModel;

    // [[ Draenei ]] \\ 4
    public GameObject draeneiMaleModel;
    public GameObject draeneiFemaleModel;

    // [[ Worgen ]] \\ 5
    public GameObject worgenFemaleModel;
    public GameObject worgenMaleModel;

    // [[ Alliance Panda ]] \\ 6
    public GameObject apandaMaleModel;
    public GameObject apandaFemaleModel;

    // [[ Orc ]] \\ 7
    public GameObject orcFemaleModel;
    public GameObject orcMaleModel;

    // [[ Undead ]] \\ 8
    public GameObject undeadMaleModel;
    public GameObject undeadFemaleModel;

    // [[ Tauren ]] \\ 9 
    public GameObject taurenFemaleModel;
    public GameObject taurenMaleModel;

    // [[ Troll ]] \\ 10
    public GameObject trollMaleModel;
    public GameObject trollFemaleModel;

    // [[ Blood Elf ]] \\ 11
    public GameObject belfFemaleModel;
    public GameObject belfMaleModel;

    // [[ Goblin ]] \\ 12
    public GameObject goblinMaleModel;
    public GameObject goblinFemaleModel;

    // [[ Horde Panda ]] \\ 12
    public GameObject hpandaMaleModel;
    public GameObject hPandaFemaleModel;

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

    public GameObject LoadCharacterModel(int raceId, int bodyType)
    {
        switch (raceId)
        {
            case 0: // null
                return null;
            case 1: // human
                if (bodyType == 1)
                    return humanMaleModel;
                else
                    return humanFemaleModel;
            case 2: // dwarf
                if (bodyType == 1)
                    return dwarfMaleModel;
                else
                    return dwarfFemaleModel;
            case 3: // night elf
                if (bodyType == 1)
                    return nelfMaleModel;
                else
                    return nelfFemaleModel;
            case 4: // draenei
                if (bodyType == 1)
                    return draeneiMaleModel;
                else
                    return draeneiFemaleModel;
            case 5: // worgen
                if (bodyType == 1)
                    return worgenMaleModel;
                else
                    return worgenFemaleModel;
            case 6: // alliance panda
                if (bodyType == 1)
                    return apandaMaleModel;
                else
                    return apandaFemaleModel;
            case 7: // Orc
                if (bodyType == 1)
                    return orcMaleModel;
                else
                    return orcFemaleModel;
            case 8: // undead
                if (bodyType == 1)
                    return undeadMaleModel;
                else
                    return undeadFemaleModel;
            case 9: // tauren
                if (bodyType == 1)
                    return taurenMaleModel;
                else
                    return taurenFemaleModel;
            case 10: // troll
                if (bodyType == 1)
                    return trollMaleModel;
                else
                    return trollFemaleModel;
            case 11: // Blood Elf
                if (bodyType == 1)
                    return belfMaleModel;
                else
                    return belfFemaleModel;
            case 12: // goblin
                if (bodyType == 1)
                    return goblinMaleModel;
                else
                    return goblinFemaleModel;
            case 13: // horde panda
                if (bodyType == 1)
                    return hpandaMaleModel;
                else
                    return hPandaFemaleModel;
        }

        return null;
    }

    public GameObject LoadCharacterModel(Character character)
    {
        switch (character.raceId)
        {
            case 0: // null
                break;
            case 1: // human
                if (character.bodyType == 1)
                    return humanMaleModel;
                else
                    return humanFemaleModel;
            case 2: // dwarf
                if (character.bodyType == 1)
                    return dwarfMaleModel;
                else
                    return dwarfFemaleModel;
            case 3: // night elf
                if (character.bodyType == 1)
                    return nelfMaleModel;
                else
                    return nelfFemaleModel;
            case 4: // draenei
                if (character.bodyType == 1)
                    return draeneiMaleModel;
                else
                    return draeneiFemaleModel;
            case 5: // worgen
                if (character.bodyType == 1)
                    return worgenMaleModel;
                else
                    return worgenFemaleModel;
            case 6: // alliance panda
                if (character.bodyType == 1)
                    return apandaMaleModel;
                else
                    return apandaFemaleModel;
            case 7: // Orc
                if (character.bodyType == 1)
                    return orcMaleModel;
                else
                    return orcFemaleModel;
            case 8: // undead
                if (character.bodyType == 1)
                    return undeadMaleModel;
                else
                    return undeadFemaleModel;
            case 9: // tauren
                if (character.bodyType == 1)
                    return taurenMaleModel;
                else
                    return taurenFemaleModel;
            case 10: // troll
                if (character.bodyType == 1)
                    return trollMaleModel;
                else
                    return trollFemaleModel;
            case 11: // Blood Elf
                if (character.bodyType == 1)
                    return belfMaleModel;
                else
                    return belfFemaleModel;
            case 12: // goblin
                if (character.bodyType == 1)
                    return goblinMaleModel;
                else
                    return goblinFemaleModel;
            case 13: // horde panda
                if (character.bodyType == 1)
                    return hpandaMaleModel;
                else
                    return hPandaFemaleModel;
        }

        return null;
    }
}
