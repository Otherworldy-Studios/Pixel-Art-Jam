using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector.Editor.TypeSearch;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;



public class TurnManager : Singleton<TurnManager>
{
    public Turn turn;
    [SerializeField] public PlayerStats[] players;
    [SerializeField] public PlayerStats player;
    [SerializeField] public PlayerStats enemy;
    [SerializeField] private GameObject turnMessage;
    [SerializeField] private TMP_Text turnMessageText;
    [SerializeField] private Color playerTurnMessageColor;
    [SerializeField] private Color enemyTurnMessageColor;
    public event Action OnTurnStart;
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
        player.hasDrawn = false;
        OnTurnStart?.Invoke();
        player.currentMana = player.maxMana;
        StartCoroutine(ShowTurnMessage());
        foreach (CardInstance cardOnBoard in player.cardsInPlay)
        {
            cardOnBoard.ResetFlags();
            cardOnBoard.DoStatusEffects();
           
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
        OnTurnStart?.Invoke();
        enemy.currentMana = enemy.maxMana;
        yield return StartCoroutine(ShowTurnMessage());
        foreach (CardInstance cardOnBoard in enemy.cardsInPlay)
        {
            cardOnBoard.ResetFlags();
            cardOnBoard.DoStatusEffects();
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
                StartCoroutine(GameManager.Instance.PlayCard(card, false));
                yield return new WaitForSeconds(0.5f);
            }
        }
        enemy.hand.Clear();
        yield return new WaitForSeconds(1.5f);
        List<CardInstance> cardsToPlay = new List<CardInstance>(enemy.cardsInPlay);
        foreach (CardInstance card in cardsToPlay)
        {
            if (player.cardsInPlay.Count > 0 )
            {
                CardInstance target = null;
                if (card.card.canTargetAllies)
                {
                    if (card.card.canOnlyTargetAllies)
                    {
                        target = enemy.cardsInPlay[Random.Range(0, enemy.cardsInPlay.Count)];
                    }
                    else
                    {
                        List<CardInstance> targets = new List<CardInstance>(player.cardsInPlay);
                        targets.AddRange(enemy.cardsInPlay);
                        target = targets[Random.Range(0, targets.Count)];
                    }
                }
                else
                {
                  target = player.cardsInPlay[Random.Range(0, player.cardsInPlay.Count)];
                }
                
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
                    yield return new WaitForSeconds(2f);
                    Debug.Log($"Special for {card.card.cardName} was successful");
                }

            }
            else
            {
                if (card == null)
                {
                    Debug.LogError("Card is null");
                    continue;
                }
                card.Attack(enemy, null);
                yield return new WaitForSeconds(1f);
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
