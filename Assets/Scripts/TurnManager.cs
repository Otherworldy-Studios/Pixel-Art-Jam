using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TurnManager : Singleton<TurnManager>
{
    public Turn turn;
    [SerializeField] public PlayerStats[] players;
    [SerializeField] private PlayerStats player;
    [SerializeField] private PlayerStats enemy;

    public void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        player.deck.Initialize();
        enemy.deck.Initialize();
        PlayerTurn();
        DealInitialHand();
    }

    public void DealInitialHand()
    {
        player.deck.DrawCards(3);
        enemy.deck.DrawCards(3);
    }

    public void PlayerTurn()
    {
        turn = Turn.Player;
    }

    public void StartEnemyTurn()
    {
        StartCoroutine(EnemyTurn());
    }
    public IEnumerator EnemyTurn()
    {
        turn = Turn.Enemy;
        int failures = 0;
        foreach (CardInstance card in enemy.hand)
        {
            if(card.card is UndeadCard undeadCard)
            {
                if(card == null)
                {
                    Debug.LogError("Card is null");
                    continue;
                }
                GameManager.Instance.PlayCard(card, false);
            }
        }
        enemy.hand.Clear();
        yield return new WaitForSeconds(3f);
        List<CardInstance> cardsToPlay = enemy.cardsInPlay;
        foreach (CardInstance card in cardsToPlay)
        {
            if (player.cardsInPlay.Count > 0)
            {
                CardInstance target = player.cardsInPlay[Random.Range(0, player.cardsInPlay.Count)];
                if(card == null)
                {
                    Debug.LogError("Card is null");
                    continue;
                }
                if (target == null)
                {
                    Debug.LogError("Target is null");
                    continue;
                }
                if (!card.DoSpecial(enemy, target))
                {
                    Debug.Log("Failed to play special");
                    failures++;
                    Debug.Log($"Special Failed for {card.card.cardName}, attacking {target} instead");
                    card.Attack(enemy,target);
                    yield return new WaitForSeconds(1f);
                    continue;
                }
                else
                {
                    Debug.Log($"Special for {card.card.cardName} was successful");
                }

            }
           
         
        }
        PlayerTurn();
       

    }
}

public enum Turn
{
    Player,
    Enemy
}
