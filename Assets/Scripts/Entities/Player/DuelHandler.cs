using UnityEngine;

public class DuelHandler : MonoBehaviour
{
    public static DuelHandler Instance { get; private set; }
    public GameObject duelPrefab;

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
    public Duel CreateDuel(Unit player1, Unit player2)
    {
        if (duelPrefab != null)
        {
            GameObject duelObject = Instantiate(duelPrefab, Vector3.zero, Quaternion.identity);
            Duel duel = duelObject.GetComponent<Duel>();
            if (duel != null)
            {
                duel.RequestDuel(player1, player2);
            }

            return duel;
        }

        return null;
    }
}
