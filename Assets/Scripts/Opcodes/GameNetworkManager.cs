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
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class GameNetworkManager : NetworkManager
{
    private OpcodeHandler opcodeHandler;
    public GameObject spellPrefab;
    public GameObject triggerPrefab;
    public Transform spawnPoint;
    public override void OnStartServer()
    {
        opcodeHandler = new OpcodeHandler();

        base.OnStartServer();

        // Register opcode handlers
        opcodeHandler.RegisterHandler(Opcodes.CMSG_CAST_SPELL,          HandleCastSpell);
        opcodeHandler.RegisterHandler(Opcodes.CMSG_SELECT_TARGET,       HandleSelectTarget);
        opcodeHandler.RegisterHandler(Opcodes.CMSG_JOIN_WORLD,          HandleJoinWorld);
        //opcodeHandler.RegisterHandler((int)Opcodes.MoveCharacter, HandleMoveCharacter);

        NetworkServer.RegisterHandler<OpcodeMessage>(OnOpcodeMessageReceived, true);
    }

    private void HandleJoinWorld(NetworkConnection conn, NetworkReader reader)
    {
        if (conn is NetworkConnectionToClient clientConn)
        {
            GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            NetworkServer.AddPlayerForConnection(clientConn, player);

            Unit playerUnit = player.GetComponent<Unit>();
            if (playerUnit != null)
            {
                playerUnit.InitBars(conn.identity);
            }
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


    private void HandleSelectTarget(NetworkConnection conn, NetworkReader reader)
    {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        Unit target = identity.GetComponent<Unit>();

        Unit player = conn.identity.GetComponent<Unit>();

        player.SetTarget(target);
    }
    private void HandleMoveCharacter(NetworkConnection conn, NetworkReader reader)
    {
        Vector3 position = reader.ReadVector3();
        Debug.Log($"Server: Move character to {position}");
        // Implement movement logic here
    }
}
