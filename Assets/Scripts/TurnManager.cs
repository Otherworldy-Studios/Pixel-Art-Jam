using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;


public class TurnManager : Singleton<TurnManager>
{
    public Turn turn;
    [SerializeField] public PlayerStats[] players;
    [SerializeField] private PlayerStats player;
    [SerializeField] private PlayerStats enemy;
    [SerializeField] private GameObject turnMessage;
    [SerializeField] private TMP_Text turnMessageText;
    [SerializeField] private Color playerTurnMessageColor;
    [SerializeField] private Color enemyTurnMessageColor;

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

    IEnumerator ShowTurnMessage()
    {
        if(turn == Turn.Player)
        {
            turnMessageText.color = playerTurnMessageColor;
            turnMessageText.text = "Player Turn";
        }
        else
        {
            turnMessageText.color = enemyTurnMessageColor;
            turnMessageText.text = "Enemy Turn";
        }
        turnMessage.SetActive(true);
        turnMessage.transform.DOPunchScale(Vector3.one * 1.5f, 0.4f, 1, 1);
        yield return new WaitForSeconds(0.5f);
        turnMessageText.DOFade(0, 0.5f);
        turnMessage.SetActive(false);
    }

    public void PlayerTurn()
    {
        turn = Turn.Player;
        StartCoroutine(ShowTurnMessage());
        foreach (CardInstance cardOnBoard in player.cardsInPlay)
        {
            cardOnBoard.ResetFlags();
        }
        foreach (Transform handPos in enemy.deck.handPositions)
        {
            if ((handPos.childCount == 0))
            {
                StartCoroutine(enemy.deck.DrawCards(3, 0.5f));
            }
        }
    }

    
    public void StartEnemyTurn()
    {
        StartCoroutine(EnemyTurn());
    }
    public IEnumerator EnemyTurn()
    {
        turn = Turn.Enemy;
        yield return StartCoroutine(ShowTurnMessage());
        foreach (CardInstance cardOnBoard in enemy.cardsInPlay)
        {
            cardOnBoard.ResetFlags();
        }
        foreach (Transform handPos in player.deck.handPositions)
        {
            if ((handPos.childCount == 0))
            {
                player.deck.DrawCards(1);
            }
        }
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
                yield return new WaitForSeconds(0.5f);
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
                    yield return new WaitForSeconds(1f);
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
