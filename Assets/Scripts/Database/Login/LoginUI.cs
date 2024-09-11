using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace
using Mirror;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField accountNameInput; // Use TMP_InputField instead of InputField
    public TMP_InputField accountPassInput;
    public Button loginButton;

    private void Start()
    {
        loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    private void OnLoginButtonClicked()
    {
        string username = accountNameInput.text;
        string password = accountPassInput.text;

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
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
    }
}
