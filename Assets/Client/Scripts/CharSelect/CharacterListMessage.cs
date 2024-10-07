using Mirror;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterListMessage : NetworkMessage
{
    public List<CharacterData> Characters;

    public struct CharacterData
    {
        public int Id;
        public string CharacterName;
        public int ClassId;
        public int SpecId;
        public int FactionId;
        public int RaceId;
        public int BodyType;
        public GameObject Model;

        public CharacterData(int id, string characterName, int classId, int specId, int factionId, int raceId, int bodyType, GameObject model)
        {
            Id = id;
            CharacterName = characterName;
            ClassId = classId;
            SpecId = specId;
            FactionId = factionId;
            RaceId = raceId;
            BodyType = bodyType;
            Model = model;
        }
    }
}
