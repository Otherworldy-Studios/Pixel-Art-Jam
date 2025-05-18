using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using DG.Tweening;
using Sirenix.Utilities;
using TMPro;
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

    public int maxMana = 10;
    public int currentMana = 0;

    public int maxHealth = 30;
    public int currentHealth = 30;

    public TMP_Text healthText;
    public TMP_Text manaText;

    public Camera Camera;
    public Canvas gameCanvas;

    [SerializeField] Transform[] thingsToShake;

    public bool hasDrawn = false;

    public string playerName;
    public void OnEnable()
    {
        currentMana = maxMana;
        currentHealth = maxHealth;
    }

    public void Update()
    {
        healthText.text = currentHealth.ToString();
        if(manaText != null)
        {
            manaText.text = currentMana.ToString();
        }
         
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        ShakeScreen();
        if (currentHealth <= 0)
        {
            Debug.Log("Game Over");
            GameManager.Instance.GameOver(isPlayer);
        }
    }

    public void ShakeScreen()
    {
      
        foreach (Transform thing in thingsToShake)
        {
            if (thing == gameCanvas.transform)
            {
                continue;
            }
            thing.DOShakePosition(0.5f, 10, 20, 90);
        }
        
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
      
    }

}
