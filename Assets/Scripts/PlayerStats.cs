using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;
using static Utility;

public class PlayerStats : MonoBehaviour
{
  
    public CardStack deck;
    public List<CardInstance> hand = new();
    public List<CardInstance> cardsInPlay = new();
    public CardStack discard;
    public Turn assignedTurn;
    private int maxHandCount = 3;
    public event Action CardPositionChanged;
    public bool isPlayer;
    public bool isMyTurn => TurnManager.Instance.turn == assignedTurn;

    public void OnEnable()
    {
        CardPositionChanged += AssignPositions;
    }

   

    public void AssignPositions()
    {
        foreach(CardInstance card in hand)
        {
           card.currentPosition = isPlayer ? CardPosition.PlayerHand : CardPosition.EnemyHand;
        }

        foreach (CardInstance card in discard.cardsInStack)
        {
            card.currentPosition = CardPosition.Discard;
        }
        foreach (CardInstance card in deck.cardsInStack)
        {
            card.currentPosition = CardPosition.Deck;
        }


    }

   
    public void OnDisable()
    {
        CardPositionChanged -= AssignPositions;
    }

}
