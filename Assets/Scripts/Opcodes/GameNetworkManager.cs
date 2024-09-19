/*
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License as published by the
 * Free Software Foundation; either version 2 of the License, or (at your
 * option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
 * more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with this program. If not, see <http://www.gnu.org/licenses/>.
 */

// Filename: GameNetworkManager.cs
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameNetworkManager : NetworkManager
{
    private OpcodeHandler opcodeHandler;
    public GameObject spellPrefab;
    public GameObject triggerPrefab;
    public Transform spawnPoint;
    public GameObject buttonPrefab;
    private NetworkManagerHUD hud;
    private bool autoStartServer = true;
    private Dictionary<NetworkConnectionToClient, (GameObject, int)> playerReferences = new Dictionary<NetworkConnectionToClient, (GameObject, int)>();
    public override void Start()
    {
        base.Start();

        // Automatically start the server when in the Editor or Standalone build
        if (autoStartServer && Application.isEditor)
        {
            StartServer();
            Debug.Log("Server started automatically.");
        }
    }

    public override void OnStartServer()
    {
        opcodeHandler = new OpcodeHandler();

        base.OnStartServer();

        SceneManager.LoadScene("SampleScene");

        // Register opcode handlers
        opcodeHandler.RegisterHandler(Opcodes.CMSG_LOGIN_REQUEST,       HandleLoginRequest);
        opcodeHandler.RegisterHandler(Opcodes.CMSG_CAST_SPELL,          HandleCastSpell);
        opcodeHandler.RegisterHandler(Opcodes.CMSG_SELECT_TARGET,       HandleSelectTarget);
        opcodeHandler.RegisterHandler(Opcodes.CMSG_JOIN_WORLD,          HandleJoinWorld);
        opcodeHandler.RegisterHandler(Opcodes.CMSG_UPDATE_POS,          HandleMoveCharacter);

        NetworkServer.RegisterHandler<OpcodeMessage>(OnOpcodeMessageReceived, true);
    }
    private void HandleJoinWorld(NetworkConnection conn, NetworkReader reader)
    {
        // Retrieve accountId from the assigned authenticationData
        int accountId = (int)conn.authenticationData; // Ensure authenticationData was correctly set on login
        int charId = reader.ReadInt();

        // Validate if character belongs to the account
        Character character = DatabaseManager.Instance.GetCharacterById(charId);
        if (character != null && character.accountId == accountId)
        {
            // Store the character ID in the connection for use in spawning
            conn.authenticationData = charId; // Assign charId to use in SpawnCharacter

            // Start the timer to spawn the character after 1.5 seconds
            Invoke(nameof(SpawnCharacter), 1.5f);
        }
        else
        {
            Debug.LogError("Character validation failed.");
        }
    }

    private void SpawnCharacter()
    {
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            // Retrieve character ID from the connection's authentication data
            int characterId = (int)conn.authenticationData;
            
            // Fetch the character's location data from the database
            CharacterLocation location = DatabaseManager.Instance.GetCharacterLocation(characterId);
            int factionId = DatabaseManager.Instance.GetFactionId(characterId);
            // Set default spawn position if no location data is found
            Vector3 spawnPosition = spawnPoint.position;
            Quaternion spawnRotation = spawnPoint.rotation;

            if (location != null)
            {
                // Use the location data if available
                spawnPosition = new Vector3(location.x, location.y, location.z);
                spawnRotation = Quaternion.Euler(0, location.orientation, 0);
            }

            // Instantiate the player at the correct location
            GameObject player = Instantiate(playerPrefab, spawnPosition, spawnRotation);

            // Ensure the player object has a Unit component
            Unit playerUnit = player.GetComponent<Unit>();
            if (playerUnit != null)
            {
                playerUnit.factionId = factionId;
                playerUnit.character = DatabaseManager.Instance.GetCharacterById(characterId);
            }

            // Add the player to the server
            playerReferences[conn] = (player, characterId);
            NetworkServer.AddPlayerForConnection(conn, player);

        }
    }

    private void OnOpcodeMessageReceived(NetworkConnection conn, OpcodeMessage msg)
    {
        opcodeHandler.HandleOpcode(conn, msg.opcode, new NetworkReader(msg.payload));
    }

    private void HandleCastSpell(NetworkConnection conn, NetworkReader reader)
    {
        int spellId = reader.ReadInt();
        Unit caster = conn.identity.GetComponent<Unit>();
        Vector3 position = reader.ReadVector3();
        if (position != Vector3.zero)
        {
            caster.CreateSpellAndPrepare(spellId, spellPrefab, triggerPrefab, position);
            return;
        }

        Spell spell = caster.CreateSpellAndPrepare(spellId, spellPrefab, triggerPrefab);
    }

    private void HandleLoginRequest(NetworkConnection conn, NetworkReader reader)
    {
        string username = reader.ReadString();
        string password = reader.ReadString();

        // Check the credentials against the database
        Account account = DatabaseManager.Instance.GetAccountByUsername(username);

        if (account != null && account.accountPass == password)
        {
            Debug.Log("Account found!");

            // Assign accountId to the connection object
            conn.authenticationData = account.Id; // Store the account ID in the connection's authenticationData

            // Send account info to client
            NetworkWriter writer = new NetworkWriter();
            writer.WriteInt(account.Id);
            writer.WriteString(account.accountName);

            OpcodeMessage accountInfoMsg = new OpcodeMessage
            {
                opcode = Opcodes.SMSG_ACCOUNT_INFO,
                payload = writer.ToArray()
            };

            conn.Send(accountInfoMsg);

            // Send character list to client
            List<Character> characters = DatabaseManager.Instance.GetCharactersByAccountId(account.Id);
            SendCharacterList(conn, characters);
        }
        else
        {
            Debug.Log("Account not found or password incorrect.");
        }
    }


    private void SendCharacterList(NetworkConnection conn, List<Character> characters)
    {
        CharacterListMessage msg = new CharacterListMessage { Characters = new List<CharacterListMessage.CharacterData>() };

        // Populate message with character data
        foreach (var character in characters)
        {
            var data = new CharacterListMessage.CharacterData(
                character.characterId,
                character.characterName,
                character.classId,
                character.specId,
                character.factionId
            );

            msg.Characters.Add(data);
        }

        // Send the character list message to the client
        conn.Send(msg);
    }








    private void HandleSelectTarget(NetworkConnection conn, NetworkReader reader)
    {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        Unit target = identity.GetComponent<Unit>();

        Unit player = conn.identity.GetComponent<Unit>();

        player.SetTarget(target);
    }
    private void HandleMoveCharacter(NetworkConnection conn, NetworkReader reader)
    {
        // Read the new position and rotation from the message
        Vector3 position = reader.ReadVector3();
        Quaternion rotation = reader.ReadQuaternion();

        // Get the game object associated with this connection
        NetworkIdentity playerIdentity = conn.identity;

        // Prepare the message to send to all clients
        NetworkWriter writer = new NetworkWriter();
        writer.WriteNetworkIdentity(playerIdentity);
        writer.WriteVector3(position);
        writer.WriteQuaternion(rotation);

        // Create the opcode message with the updated position and rotation
        OpcodeMessage moveMessage = new OpcodeMessage
        {
            opcode = Opcodes.SMSG_UPDATE_POS,
            payload = writer.ToArray()
        };

        // Send the message to all clients including the sender
        NetworkServer.SendToAll(moveMessage);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (playerReferences.TryGetValue(conn, out var playerData))
        {
            GameObject player = playerData.Item1;
            int characterId = playerData.Item2;

            if (player != null)
            {
                Vector3 position = player.transform.position;
                float orientation = player.transform.eulerAngles.y;

                CharacterLocation location = new CharacterLocation
                {
                    Id = characterId,
                    x = position.x,
                    y = position.y,
                    z = position.z,
                    orientation = orientation
                };

                DatabaseManager.Instance.UpdateCharacterLocation(location);
                Debug.Log($"Character {characterId} location saved to the database.");
            }

            playerReferences.Remove(conn);
        }

        base.OnServerDisconnect(conn);
    }



    public override void OnStartClient()
    {
        base.OnStartClient();
        if (hud != null)
        {
            hud.gameObject.SetActive(false); // Hide the HUD for the client
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (hud != null)
        {
            hud.gameObject.SetActive(true); // Optionally re-enable the HUD if needed
        }
    }

}
