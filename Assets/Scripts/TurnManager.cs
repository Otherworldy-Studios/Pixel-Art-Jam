using UnityEngine;

public class TurnManager : Singleton<TurnManager>
{
    public PlayerTurn playerTurn;
    [SerializeField] public PlayerStats[] players;
    [SerializeField] private PlayerStats player1;
    [SerializeField] private PlayerStats player2;

    public void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        players = GameObject.FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        player1 = players[0];
        player1.assignedTurn = PlayerTurn.Player1;
        player2 = players[1];
        player2.assignedTurn = PlayerTurn.Player2;
        playerTurn = PlayerTurn.Player1;
        foreach (PlayerStats player in players)
        {
            player.Initialize();
        }
        DealInitialHand();
    }

    public void DealInitialHand()
    {
        player1.DrawCards(2);
        player2.DrawCards(2);
    }
}

public enum PlayerTurn
{
    Player1,
    Player2
}
