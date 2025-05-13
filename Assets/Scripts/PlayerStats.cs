using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;
using static Utility;

public class PlayerStats : MonoBehaviour
{
    public List<CardSO> allCards;
    public List<CardSO> deck = new();
    public List<CardInstance> hand = new();
    public List<CardInstance> discard = new();
    public PlayerTurn assignedTurn;
    private int maxHandCount = 3;
    public event Action CardPositionChanged;
    public bool isPlayer;
    public bool isMyTurn => TurnManager.Instance.playerTurn == assignedTurn;

    public void OnEnable()
    {
        CardPositionChanged += AssignPositions;
    }

    public void Initialize()
    {
        
        ShuffleDeck();
        for (int i = 0; i < allCards.Count; i++)
        {
            hand[i].Initialize(deck[i]);
            if (isPlayer)
            {
                hand[i].isPlayerCard = true;
            }
            else
            {
                hand[i].isPlayerCard = false;
            }
            
        }
        
       
    }

    public void DrawCards(int amount)
    {
        if(amount > maxHandCount)
        {
            Debug.LogError($"Tried to draw more than{maxHandCount} which is the max amount of cards allowed in hand");
            return;
        }
        for (int i = 0; i < amount; i++)
        {
            if (amount > deck.Count)
            {
                Debug.LogError($"Not enough cards in deck to draw {amount} cards \n Current Deck count is {deck.Count}");
                return;
            }
            hand[i].Initialize(deck[i]);
        }
        CardPositionChanged?.Invoke();

    }

    public void AssignPositions()
    {
        foreach(CardInstance card in hand)
        {
           card.currentPosition = isPlayer ? CardPosition.PlayerHand : CardPosition.EnemyHand;
        }

        foreach (CardInstance card in discard)
        {
            card.currentPosition = CardPosition.Discard;
        }

       
    }

    public void ShuffleDeck()
    {
        deck.Shuffle();
    }

    public void OnDisable()
    {
        CardPositionChanged -= AssignPositions;
    }

}
