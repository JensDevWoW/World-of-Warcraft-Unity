using UnityEngine;

public class Duel : MonoBehaviour
{
    public Unit player1;
    public Unit player2;
    public Vector3 duelStartPoint1;  // Starting location for player1
    public Vector3 duelStartPoint2;  // Starting location for player2
    public float allowedDistance = 100f;  // Distance allowed from the start point
    public float outOfBoundsTimer = 10f;  // Time allowed to return before losing the duel
    private float timerPlayer1 = 0f;
    private float timerPlayer2 = 0f;
    private bool isPlayer1OutOfBounds = false;
    private bool isPlayer2OutOfBounds = false;
    private bool hasStarted = false;
    private bool hasInitiated = false;
    private bool waitingForResponse = false;
    private float requestTimer = 30f;
    public float duelStartTimer = 3f;

    void Update()
    {
        if (waitingForResponse && !hasStarted)
        {
            requestTimer -= Time.deltaTime;
            if (requestTimer <= 0)
            {
                EndDuel(null, "Duel request timed out.");
            }
        }

        if (hasInitiated && !hasStarted)
        {
            duelStartTimer -= Time.deltaTime;
            if (duelStartTimer <= 0)
            {
                StartDuel();
            }
        }

        if (hasStarted)
        {
            // Check distance of both players from their starting positions
            CheckOutOfBounds(player1, duelStartPoint1, ref isPlayer1OutOfBounds, ref timerPlayer1);
            CheckOutOfBounds(player2, duelStartPoint2, ref isPlayer2OutOfBounds, ref timerPlayer2);

            if (isPlayer1OutOfBounds && timerPlayer1 <= 0)
            {
                EndDuel(player2, $"{player1.name} was too far from the duel area and lost.");
            }
            else if (isPlayer2OutOfBounds && timerPlayer2 <= 0)
            {
                EndDuel(player1, $"{player2.name} was too far from the duel area and lost.");
            }

            // Health check for win condition
            if (player1.GetHealth() <= 0)
            {
                EndDuel(player2, $"{player2.name} wins the duel!");
            }
            else if (player2.GetHealth() <= 0)
            {
                EndDuel(player1, $"{player1.name} wins the duel!");
            }
        }
    }

    public bool HasStarted()
    {
        return hasStarted;
    }

    private void CheckOutOfBounds(Unit player, Vector3 startPoint, ref bool isOutOfBounds, ref float timer)
    {
        float distanceFromStart = Vector3.Distance(player.transform.position, startPoint);

        if (distanceFromStart > allowedDistance)
        {
            if (!isOutOfBounds)
            {
                isOutOfBounds = true;
                timer = outOfBoundsTimer;  // Start the countdown timer
                Debug.Log($"{player.name} has moved too far! Timer started.");
            }
            else
            {
                timer -= Time.deltaTime;  // Decrease the timer while out of bounds
                Debug.Log($"{player.name} has {timer:F1} seconds to return.");
            }
        }
        else
        {
            if (isOutOfBounds)
            {
                Debug.Log($"{player.name} has returned to the duel area.");
            }
            isOutOfBounds = false;
            timer = outOfBoundsTimer;  // Reset the timer if they return within range
        }
    }

    public Player GetPlayer(Player player)
    {
        if (player == player1.ToPlayer())
            return player1.ToPlayer();
        else if (player == player2.ToPlayer())
            return player2.ToPlayer();

        return null;
    }

    public void RequestDuel(Unit requester, Unit target)
    {
        player1 = requester;
        player2 = target;
        duelStartPoint1 = player1.transform.position;
        duelStartPoint2 = player2.transform.position;
        waitingForResponse = true;
        Debug.Log($"{player1.name} has challenged {player2.name} to a duel!");
    }

    public void RespondToDuel(bool accepted)
    {
        if (!waitingForResponse) return;

        if (accepted)
        {
            Debug.Log($"{player2.name} accepted the duel.");
            waitingForResponse = false;
            Begin();
        }
        else
        {
            Debug.Log($"{player2.name} declined the duel.");
            EndDuel(null, "Duel declined.");
        }
    }

    private void Begin()
    {
        hasInitiated = true;
        Debug.Log("Duel initiation timer started (3 seconds).");
    }

    private void StartDuel()
    {
        hasStarted = true;
        Debug.Log("Duel has started!");
    }

    private void EndDuel(Unit winner, string message)
    {
        Debug.Log(message);
        Destroy(gameObject);  // Clean up the duel instance
    }
}
