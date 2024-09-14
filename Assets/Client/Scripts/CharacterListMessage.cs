using Mirror;
using System.Collections.Generic;

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

        public CharacterData(int id, string characterName, int classId, int specId, int factionId)
        {
            Id = id;
            CharacterName = characterName;
            ClassId = classId;
            SpecId = specId;
            FactionId = factionId;
        }
    }
}
