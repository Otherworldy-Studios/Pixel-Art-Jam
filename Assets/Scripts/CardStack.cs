using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardStack : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] PlayerStats playerStats;
    [SerializeField] Vector3 targetViewRot;
    [SerializeField] Vector3 targetViewScale;
    [SerializeField] CardPosition cardPosition = CardPosition.Deck;
    [SerializeField] GameObject cardPrefab;
    public List<CardInstance> cardsInStack = new();
    private int maxHandCount = 3;
    [SerializeField] private int maxDeckCount = 40;
    [SerializeField] private Vector3 stackPosition;
    [SerializeField] private Vector3 stackRotation;
    public Transform[] handPositions;
    public int OccupiedHandPositions = -1;
    [SerializeField] private Canvas displayCanvas;
    [SerializeField] private bool isPlayerStack;
    




    public void Initialize()
    {
        if (cardPosition == CardPosition.Deck)
        {
            InitializeDeck();
        }

        for (int i = 0; i < maxDeckCount; i++)
        {
            int roll = Random.Range(0, GameManager.Instance.allCards.Count);
            cardsInStack[i].Initialize(GameManager.Instance.allCards[roll]);
           
            if (playerStats.isPlayer)
            {
                cardsInStack[i].isPlayerCard = true;
                cardsInStack[i].gameObject.name = $"Player {GameManager.Instance.allCards[roll]} {i}";
            }
            else
            {
                cardsInStack[i].isPlayerCard = false;
                cardsInStack[i].gameObject.name = $"Enemy {GameManager.Instance.allCards[roll]}  + i";
            }


        }
        if (cardPosition == CardPosition.Deck)
        {
            ShuffleStack();
        }


    }

    void InitializeDeck()
    {
        for (int i = 0; i < maxDeckCount; i++)
        {
            GameObject card = Instantiate(cardPrefab, transform.parent);
            CardInstance cardInstance = card.GetComponent<CardInstance>();
            
            cardsInStack.Add(cardInstance);
            cardsInStack[i].owner = playerStats;
            cardInstance.currentPosition = CardPosition.Deck;
            cardInstance.originalParent = transform.parent;
            cardInstance.displayCanvas = displayCanvas;
            
        }
    }

    public void DrawCards(int amount)
    {
       
        if (OccupiedHandPositions < 0)
        {
            OccupiedHandPositions = 0;
        }
        if (playerStats.hand.Count >= maxHandCount)
        {
            Debug.LogError("Max hand count reached");
            return;
        }
        if (cardsInStack.Count == 0)
        {
            Debug.LogError("No cards left in the deck");
            return;
        }
        for (int i = 0; i < amount; i++)
        {
            if (cardsInStack.Count == 0)
            {
                Debug.LogError("No cards left in the deck");
                return;
            }
            DrawCard();
        }
        
    }

    public IEnumerator DrawCards(int amount, float delay)
    {
        if (OccupiedHandPositions < 0)
        {
            OccupiedHandPositions = 0;
        }
        if (playerStats.hand.Count >= maxHandCount)
        {
            Debug.LogError("Max hand count reached");
            yield break;
        }
        if (cardsInStack.Count == 0)
        {
            Debug.LogError("No cards left in the deck");
            yield break;
        }
        for (int i = 0; i < amount; i++)
        {
            if (cardsInStack.Count == 0)
            {
                Debug.LogError("No cards left in the deck");
                yield break;
            }
            DrawCard();
            yield return new WaitForSeconds(delay);
        }
    }

    public void DrawCard()
    {
        CardInstance card = cardsInStack[0];
        for(int i = 0; i < maxHandCount; i++)
        {
            if (handPositions[i].childCount != 0)
            {
                continue;
            }
            else
            {
                card.moving = true;
                playerStats.hand.Add(card);
                card.currentPosition = playerStats.isPlayer ? CardPosition.PlayerHand : CardPosition.EnemyHand;
                card.gameObject.transform.SetParent(handPositions[i]);
                card.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
                card.originalParent = handPositions[i];
                OccupiedHandPositions++;
                card.rectTransform.anchoredPosition = Vector3.zero;
                cardsInStack.RemoveAt(0);
                playerStats.AssignPositions();
                card.moving = false;
                break;
            }
        }
      
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isPlayerStack)
        {
            return;
        }
        if(cardPosition == CardPosition.Discard)
        {
            return;
        }
        transform.DOScale(targetViewScale, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isPlayerStack)
        {
            return;
        }
        if (cardPosition == CardPosition.Discard)
        {
            return;
        }
        transform.DOLocalRotate(Vector3.zero, 0.2f);
        transform.DOScale(Vector3.one, 0.2f);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (cardPosition == CardPosition.Deck)
        {
            DrawCards(1);
            return;
        }
    }

    public void ShuffleStack()
    {
        cardsInStack.Shuffle();
    }



}
