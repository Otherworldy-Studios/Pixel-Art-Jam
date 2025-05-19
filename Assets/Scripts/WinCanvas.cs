using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinCanvas : MonoBehaviour
{
    public PlayerStats player;
    public List<CardInstance> cardsGained = new();
    public List<CardInstance> currentDeck = new();
    public List<CardInstance> allCards = new();
    public List<CardInstance> allCardsInCanvas = new();

    private void OnEnable()
    {
        UpdateCards();
        allCardsInCanvas.AddRange(cardsGained);
        allCardsInCanvas.AddRange(currentDeck);
        allCardsInCanvas.AddRange(allCards);
    }

    public void UpdateCards()
    {
        
        foreach (CardInstance card in cardsGained)
        {
            if (card.card == null)
            {
                card.gameObject.SetActive(false);
                continue;
            }
            card.Initialize(card.card);
        }
        for (int i = 0; i < GameManager.Instance.playerCards.Count; i++)
        {
            currentDeck[i].Initialize(GameManager.Instance.playerCards[i]);
        }
        for (int i = 0; i < GameManager.Instance.allCards.Count; i++)
        {
            allCards[i].Initialize(GameManager.Instance.allCards[i]);
        }
        
    }

    public void CardHover(bool isHovering)
    {
        foreach (CardInstance card in allCardsInCanvas)
        {
            if (card == null)
            {
                continue;
            }
            card.GetComponent<LayoutElement>().ignoreLayout = isHovering;
        }
    }

    private void Update()
    {
        foreach (CardInstance card in allCardsInCanvas)
        {
            if(card == null)
            {
                continue;
            }
            if (card.card == null)
            {
               card.gameObject.SetActive(false);
            }
            else
            {
                card.gameObject.SetActive(true);
            }
        }
    }
}
