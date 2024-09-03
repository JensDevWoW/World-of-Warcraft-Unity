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

using UnityEngine;
using Mirror;

public class TargetHandler : MonoBehaviour
{
    public Unit currentTarget;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Assuming left-click for selection
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Unit unit = hit.collider.GetComponent<Unit>();

                if (unit != null)
                {
                    currentTarget = unit;
                    Debug.Log($"Target selected: {unit.name}");

                    // Send the selected target to the server for validation
                    SendTargetToServer(unit);
                    UIHandler.Instance.UpdateTarget(unit);
                }
            }
        }
    }

    void SendTargetToServer(Unit unit)
    {
        NetworkWriter writer = new NetworkWriter();

        writer.WriteNetworkIdentity(unit.GetComponent<NetworkIdentity>());
        OpcodeMessage msg = new OpcodeMessage
        {
            opcode = Opcodes.CMSG_SELECT_TARGET,
            payload = writer.ToArray()
        };

        // Send the message to the server
        NetworkClient.Send(msg);
    }
}
