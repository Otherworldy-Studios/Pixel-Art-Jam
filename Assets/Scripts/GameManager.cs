using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public List<CardSO> allCards;
    public List<CardSO> enemyCards;
    public List<CardInstance> board;
    public Transform[] playerBoardSide;
    public Transform[] enemyBoardSide;
    public Transform playerEnvironment;
    public Transform enemyEnvironment;
    public Canvas displayCanvas;

    public int OccupiedPlayerPositions = 0;
    public int OccupiedEnemyPositions = 0;

    public PlayerStats player;
    public PlayerStats enemy;

    public GameObject cardOptionsUI;
    public TMP_Text cardOptionsTitle;
    public TMP_Text cardOptionsDescription;
    public TMP_Text cardOptionsManaCostText;
    public Button attackButton;
    public Button specialButton;
    public Button discardButton;

    public bool targetSelectionMode = false;
    public bool attackSelected = false;
    public bool specialSelected = false;
    public CardInstance selectedCard;
    public CardInstance selectedTarget;

    public GameObject cardPrefab;
    public event Action<CardInstance> OnCardPlayed;


    public IEnumerator PlayCard(CardInstance card, bool isPlayer, bool fromHand = true)
    {
        card.moving = true;
        if (isPlayer)
        {
            if (OccupiedPlayerPositions >= playerBoardSide.Length)
            {
                Debug.LogError("No more positions available on the player board");
                yield break;
            }
            for (int i = 0; i < playerBoardSide.Length; i++)
            {
                if (playerBoardSide[i].childCount != 0)
                {
                    continue;
                }
                else
                {
                    card.gameObject.transform.parent = playerBoardSide[i];
                    card.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
                    card.originalParent = playerBoardSide[i];
                    card.gameObject.transform.localScale = new Vector3(1, 1, 1);
                    player.hand.Remove(card);
                    if (fromHand)
                    {
                        player.deck.OccupiedHandPositions--;
                    }
                    player.cardsInPlay.Add(card);
                    OccupiedPlayerPositions++;
                    break;
                }
            }
            
        }
        else
        {
            List<CardInstance> enemyCards = enemy.hand;
            if (OccupiedEnemyPositions >= enemyBoardSide.Length)
            {
                Debug.LogError("No more positions available on the enemy board");
                yield break;
            }

            for (int i = 0; i < enemyBoardSide.Length; i++)
            {
                if (enemyBoardSide[i].childCount != 0)
                {
                    continue;
                }
                else
                {
                    card.gameObject.transform.parent = enemyBoardSide[i];
                    card.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
                    card.originalParent = enemyBoardSide[i];
                    card.gameObject.transform.localScale = new Vector3(1, 1, 1);
                    //enemy.hand.Remove(card);
                    if (fromHand)
                    {
                        enemy.deck.OccupiedHandPositions--;
                    }
                    enemy.cardsInPlay.Add(card);
                    OccupiedEnemyPositions++;
                    break;
                }
            }

            
        }
        board.Add(card);
        card.currentPosition = CardPosition.Board;
        card.rectTransform.DOAnchorPos3D(Vector3.zero,0.5f);
        card.moving = false;
        if (card.card.hasSpecialCondition)
        {
            card.SubscribeToCardEvents();
        }
        OnCardPlayed?.Invoke(card);
        yield return null;
    }

    public void StartPlayCardCoroutine(CardInstance card, bool isPlayer, bool fromHand = true)
    {
        StartCoroutine(PlayCard(card,isPlayer,fromHand));
    }

    public void Update()
    {
        if(targetSelectionMode)
        {
            if(Input.GetMouseButtonUp(1))
            {
                ExitTargetSelection();
            }
        }
    }

    public void DiscardCard(CardInstance card, bool isPlayer)
    {
        card.gameObject.transform.localScale = Vector3.one;
        if (cardOptionsUI.activeSelf)
        {
            HideCardOptions();
        }
        card.moving = true;
        board.Remove(card);
        card.UnsubscribeToCardEvents();

        card.currentPosition = CardPosition.Discard;
        if (isPlayer)
        {
            player.discard.cardsInStack.Add(card);
            card.gameObject.transform.parent = player.discard.transform.parent;
            card.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
            card.originalParent = player.discard.transform;
            player.cardsInPlay.Remove(card);
            OccupiedPlayerPositions--;
        }
        else
        {
            enemy.discard.cardsInStack.Add(card);
            card.gameObject.transform.parent = enemy.discard.transform.parent;
            card.transform.DOLocalRotate(new Vector3(0, 180, 0), 0.5f);
            card.originalParent = enemy.discard.transform;
            enemy.cardsInPlay.Remove(card);
            OccupiedEnemyPositions--;
        }
       
        card.rectTransform.anchoredPosition = Vector3.zero;
        card.gameObject.transform.localScale = Vector3.one;
        card.moving = false;

        if(card.card is ArmoredZombie zombie)
        {
            foreach (CardInstance cardInPlay in board)
            {
                if (cardInPlay.owner == card.owner && card.modifiedByArmoredZombie)
                {
                    cardInPlay.damageModifier += zombie.damageAbsorbed;
                }
            }
        }
    }



    public void ShowCardOptions(CardInstance card)
    {
        if(targetSelectionMode)
        {
            return;
        }
        cardOptionsUI.SetActive(true);
       cardOptionsTitle.text = card.cardName.text + " Selected";
       cardOptionsManaCostText.text = card.card.manaCost.ToString();
        // cardOptionsDescription.text = card.cardDescription.text;
        attackButton.onClick.RemoveAllListeners();
       specialButton.onClick.RemoveAllListeners();
       attackButton.onClick.AddListener(() => SelectAttack(card));
       specialButton.onClick.AddListener(() => SelectSpecial(card));
       discardButton.onClick.AddListener(() => DiscardCard(card,card.isPlayerCard));
    }

    public void HideCardOptions()
    {
        cardOptionsUI.SetActive(false);
        attackButton.onClick.RemoveAllListeners();
        specialButton.onClick.RemoveAllListeners();
        
    }

    public IEnumerator EnterTargetSelection(CardInstance card , bool attack)
    {
        targetSelectionMode = true;
        Debug.Log("Target selection mode activated");
        while (targetSelectionMode)
        {
            yield return new WaitUntil(() => selectedTarget != null);
            
            if (attackSelected)
            {
                card.Attack(card.owner, selectedTarget);
                targetSelectionMode = false;
            }
            else if (specialSelected)
            {
                if (card.card.canSelectTarget)
                {
                    card.DoSpecial(card.owner, selectedTarget);
                }
               
                targetSelectionMode = false;
            }
        }
        selectedTarget = null;
        yield return null;

    }

    public void ExitTargetSelection()
    {
        targetSelectionMode = false;
        selectedTarget = null;
        attackSelected = false;
        specialSelected = false;
        HideCardOptions();
    }



    public void SelectAttack(CardInstance card)
    {
        attackSelected = true;
        specialSelected = false;
        HideCardOptions();
        if (GameManager.Instance.OccupiedEnemyPositions == 0)
        {
           card.Attack(card.owner, null);
           return;
        }
        StartCoroutine(EnterTargetSelection(selectedCard, true));
    }
    public void SelectSpecial(CardInstance card)
    {
        specialSelected = true;
        attackSelected = false;
        HideCardOptions();
        if (card.card.canSelectTarget)
        {
          
            StartCoroutine(EnterTargetSelection(selectedCard, false));
        }
        else
        {
            card.DoSpecial(card.owner, null);
        }
       
    }

    public CardInstance InstantiateCard(CardSO cardSO, PlayerStats owner, CardPosition position, Transform parent = null, List<CardInstance> listToAddTo = null)
    {
        GameObject card = Instantiate(cardPrefab, parent);
        CardInstance cardInstance = card.GetComponent<CardInstance>();
        listToAddTo?.Add(cardInstance);
        cardInstance.owner = owner;
        cardInstance.currentPosition = position;
        cardInstance.originalParent = parent;
        cardInstance.displayCanvas = displayCanvas;
        cardInstance.Initialize(cardSO);
        return cardInstance;
    }

    public void GameOver(bool isPlayer)
    {
        if (isPlayer)
        {
            Debug.Log("Game Over. Player has lost.");
        }
        else
        {
            SceneCounter.Instance.LoadNextScene();
        }
    }
}


