using UnityEngine;

public class AuthenticationManager : MonoBehaviour
{
    public DatabaseManager DatabaseManager;

    public bool AuthenticatePlayer(string username, string password, out Account account)
    {
        account = DatabaseManager.GetAccountByUsername(username);
        if (account != null && account.accountPass == password)
        {
            Debug.Log("Authentication successful.");
            return true;
        }
        Debug.LogWarning("Authentication failed.");
        return false;
    }
}
