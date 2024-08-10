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
