using Mirror;
using UnityEngine;

public class ClientAccountManager : MonoBehaviour
{
    public static ClientAccountManager Instance { get; private set; }

    public Account CurrentAccount { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Account GetCurrentAccount()
    {
        return CurrentAccount;
    }

    public void SetCurrentAccount(Account account)
    {
        CurrentAccount = account;
    }
}
