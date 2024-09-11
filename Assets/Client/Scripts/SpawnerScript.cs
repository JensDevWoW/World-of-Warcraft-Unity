using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class SpawnerScript : NetworkBehaviour
{
    public Button spawnButton;
    private void Start()
    {
        if (spawnButton != null)
        {
            spawnButton.onClick.AddListener(OnSpawnButtonClicked);
        }
    }

    private void OnSpawnButtonClicked()
    {
        if (NetworkClient.isConnected && NetworkClient.ready)
        {
            if (!HasLocalPlayer())
            {
                SendJoinWorldOpcode();
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Player is already spawned.");
            }
        }
        else
        {
            Debug.LogWarning("Client is not connected or ready to spawn a player.");
        }
    }

    private void SendJoinWorldOpcode()
    {
        NetworkWriter writer = new NetworkWriter();
        OpcodeMessage joinWorldPacket = new OpcodeMessage
        {
            opcode = Opcodes.CMSG_JOIN_WORLD,
            payload = writer.ToArray()
        };
        NetworkClient.Send(joinWorldPacket);
    }

    private bool HasLocalPlayer()
    {
        return NetworkClient.localPlayer != null;
    }
}
