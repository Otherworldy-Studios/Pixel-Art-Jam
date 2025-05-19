using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using DG.Tweening;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
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
    public GameObject damagePanel;

    [SerializeField] Transform[] thingsToShake;

    public bool hasDrawn = false;

    public string playerName;
    public void OnEnable()
    {
        healthText = isPlayer ? GameManager.Instance.playerHealthText : GameManager.Instance.enemyHealthText;
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
        StartCoroutine(DamageFlash());
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

    /// <summary>
    /// Coroutine to briefly flash the card red when damaged.
    /// </summary>
    public IEnumerator DamageFlash()
    {
        if (damagePanel == null)
        {
            yield break;
        }
        damagePanel.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        damagePanel?.SetActive(false);
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
