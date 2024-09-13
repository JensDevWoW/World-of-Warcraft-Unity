using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System.IO;
using System.Collections;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField accountNameInput;
    public TMP_InputField accountPassInput;
    public Button loginButton;

    private void Start()
    {
        loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    private void OnLoginButtonClicked()
    {
        loginButton.interactable = false; // Disable button to prevent multiple clicks
        StartCoroutine(TryConnectAndLogin());
    }

    private IEnumerator TryConnectAndLogin()
    {
        string username = accountNameInput.text;
        string password = accountPassInput.text;

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            if (TryConnectToServer())
            {
                // Wait for the connection to complete
                yield return new WaitUntil(() => NetworkClient.isConnected);

                // Create the login message
                NetworkWriter writer = new NetworkWriter();
                writer.WriteString(username);
                writer.WriteString(password);

                // Send the login opcode message to the server
                NetworkClient.Send(new OpcodeMessage
                {
                    opcode = Opcodes.CMSG_LOGIN_REQUEST,
                    payload = writer.ToArray()
                });
            }
            else
            {
                Debug.LogError("Failed to connect to the server. Check your realmlist settings.");
            }
        }
        loginButton.interactable = true; // Re-enable the button after the process is complete
    }

    private bool TryConnectToServer()
    {
        string realmlistPath = Path.Combine(Application.streamingAssetsPath, "realmlist.txt");

        if (File.Exists(realmlistPath))
        {
            string[] lines = File.ReadAllLines(realmlistPath);
            foreach (string line in lines)
            {
                if (line.StartsWith("SET realmlist"))
                {
                    string[] parts = line.Split('"');
                    if (parts.Length > 1)
                    {
                        string address = parts[1].Trim();
                        Debug.Log($"Connecting to {address}...");
                        NetworkManager.singleton.networkAddress = address;
                        NetworkManager.singleton.StartClient();

                        return true; // Attempt to connect
                    }
                }
            }
            Debug.LogError("Invalid realmlist format. Expected format: 'SET realmlist \"IPAddressHere\"'");
            return false;
        }
        else
        {
            Debug.LogError("realmlist.txt not found.");
            return false;
        }
    }
}
