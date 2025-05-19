using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the overall game state, card play, board state, and UI interactions.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    #region Fields and Properties

    // --- Card Data ---
    [Header("Card Data")]
    public List<CardSO> allCards;
    public List<CardSO> enemyCards;
    public List<CardSO> playerCards;
    public List<CardInstance> board;

    // --- Board Transforms ---
    [Header("Board Transforms")]
    public Transform[] playerBoardSide;
    public Transform[] enemyBoardSide;
    public Transform playerEnvironment;
    public Transform enemyEnvironment;

    // --- UI References ---
    [Header("UI References")]
    public Canvas displayCanvas;
    public GameObject cardOptionsUI;
    public TMP_Text cardOptionsTitle;
    public TMP_Text cardOptionsDescription;
    public TMP_Text cardOptionsManaCostText;
    public TMP_Text actionMessage;
    public Button attackButton;
    public Button specialButton;
    public Button discardButton;
    public GameObject winCanvas;
    public GameObject UICanvas;

    public TMP_Text playerHealthText;
    public TMP_Text enemyHealthText;

    // --- Game State ---
    [Header("Game State")]
    public int OccupiedPlayerPositions = 0;
    public int OccupiedEnemyPositions = 0;
    public PlayerStats player;
    public PlayerStats enemy;
    public bool targetSelectionMode = false;
    public bool attackSelected = false;
    public bool specialSelected = false;
    public CardInstance selectedCard;
    public CardInstance selectedTarget;

    // --- Prefabs ---
    [Header("Prefabs")]
    public GameObject cardPrefab;

    [Header("Combat Log UI")]
    public Transform combatLogContainer;
    public GameObject combatLogMessagePrefab;

    private readonly List<GameObject> combatLogMessages = new();
    private const int MaxCombatLogMessages = 3;
    public float messageSpacing = 32f;
    private float fadeDuration = 0.5f;

    // --- Events ---
    public event Action<CardInstance> OnCardPlayed;

    protected override void Awake()
    {
        base.Awake();
        enemyCards = SceneCounter.Instance.sceneCount switch
        {
            1 => new List<CardSO>(SceneCounter.Instance.Enemy1Cards),
            2 => new List<CardSO>(SceneCounter.Instance.Enemy2Cards),
            3 => new List<CardSO>(SceneCounter.Instance.Enemy3Cards),
            4 => new List<CardSO>(SceneCounter.Instance.Enemy4Cards),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
    #endregion
    #region Card Play & Board Management

    /// <summary>
    /// Plays a card onto the board for the player or enemy.
    /// </summary>
    /// <param name="card">The card instance to play.</param>
    /// <param name="isPlayer">True if the card is played by the player.</param>
    /// <param name="fromHand">True if the card is from hand.</param>
    public IEnumerator PlayCard(CardInstance card, bool isPlayer, bool fromHand = true)
    {
        card.moving = true;
        if (isPlayer)
        {
            if (OccupiedPlayerPositions >= playerBoardSide.Length)
            {
                Debug.LogError("No more positions available on the player board");
                card.moving = false;
                yield break;
            }
            for (int i = 0; i < playerBoardSide.Length; i++)
            {
                if (playerBoardSide[i].childCount != 0)
                    continue;

                card.gameObject.transform.parent = playerBoardSide[i];
                card.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
                card.originalParent = playerBoardSide[i];
                card.gameObject.transform.localScale = Vector3.one;
                player.hand.Remove(card);
                if (fromHand)
                    player.deck.OccupiedHandPositions--;
                player.cardsInPlay.Add(card);
                OccupiedPlayerPositions++;
                break;
            }
        }
        else
        {
            if (OccupiedEnemyPositions >= enemyBoardSide.Length)
            {
                Debug.LogError("No more positions available on the enemy board");
                yield break;
            }
            for (int i = 0; i < enemyBoardSide.Length; i++)
            {
                if (enemyBoardSide[i].childCount != 0)
                    continue;

                card.gameObject.transform.parent = enemyBoardSide[i];
                card.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
                card.originalParent = enemyBoardSide[i];
                card.gameObject.transform.localScale = Vector3.one;
                //enemy.hand.Remove(card);
                if (fromHand)
                    enemy.deck.OccupiedHandPositions--;
                enemy.cardsInPlay.Add(card);
                OccupiedEnemyPositions++;
                break;
            }
        }
        board.Add(card);
        card.currentPosition = CardPosition.Board;
        card.rectTransform.DOAnchorPos3D(Vector3.zero, 0.5f);
        card.moving = false;
        if (card.card.hasSpecialCondition)
            card.SubscribeToCardEvents();
        OnCardPlayed?.Invoke(card);
        yield return null;
    }

    /// <summary>
    /// Starts the PlayCard coroutine.
    /// </summary>
    public void StartPlayCardCoroutine(CardInstance card, bool isPlayer, bool fromHand = true)
    {
        StartCoroutine(PlayCard(card, isPlayer, fromHand));
    }

    /// <summary>
    /// Discards a card from play.
    /// </summary>
    public void DiscardCard(CardInstance card, bool isPlayer)
    {
        card.gameObject.transform.localScale = Vector3.one;
        if (cardOptionsUI.activeSelf)
            HideCardOptions();
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

        // Special handling for Armored Zombie
        if (card.card is ArmoredZombie zombie)
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

    /// <summary>
    /// Instantiates a card prefab and initializes it.
    /// </summary>
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

    #endregion

    #region UI

    /// <summary>
    /// Shows the card options UI for a selected card.
    /// </summary>
    public void ShowCardOptions(CardInstance card)
    {
        if (targetSelectionMode)
            return;
        cardOptionsUI.SetActive(true);
        cardOptionsTitle.text = card.cardName.text + " Selected";
        cardOptionsManaCostText.text = card.card.manaCost.ToString();
        // cardOptionsDescription.text = card.cardDescription.text;
        attackButton.onClick.RemoveAllListeners();
        specialButton.onClick.RemoveAllListeners();
        attackButton.onClick.AddListener(() => SelectAttack(card));
        specialButton.onClick.AddListener(() => SelectSpecial(card));
        discardButton.onClick.AddListener(() => DiscardCard(card, card.isPlayerCard));
    }

    /// <summary>
    /// Hides the card options UI.
    /// </summary>
    public void HideCardOptions()
    {
        cardOptionsUI.SetActive(false);
        attackButton.onClick.RemoveAllListeners();
        specialButton.onClick.RemoveAllListeners();
    }


    private readonly Queue<string> actionMessageQueue = new();
    private bool isDisplayingActionMessage = false;


    /// <summary>
    /// Enqueue an action message to be displayed in sequence.
    /// </summary>
    public void EnqueueActionMessage(string message, float delay = 2f)
    {
        StartCoroutine(ProcessActionMessageQueue(message, delay));
    }

    private IEnumerator ProcessActionMessageQueue(string message, float delay)
    {
        GameObject msgObj = Instantiate(combatLogMessagePrefab, combatLogContainer);
        TMP_Text msgText = msgObj.GetComponent<TMP_Text>();
        msgText.text = message;
        msgText.alpha = 1f;
        msgObj.SetActive(true);


        RectTransform msgRect = msgObj.GetComponent<RectTransform>();
        msgRect.anchoredPosition = new Vector2(0, -messageSpacing * (combatLogMessages.Count));

        combatLogMessages.Add(msgObj);


        for (int i = 0; i < combatLogMessages.Count; i++)
        {
            RectTransform rect = combatLogMessages[i].GetComponent<RectTransform>();
            rect.DOAnchorPosY(-messageSpacing * (combatLogMessages.Count - 1 - i), 0.2f);
        }


        if (combatLogMessages.Count > MaxCombatLogMessages)
        {
            GameObject oldest = combatLogMessages[0];
            TMP_Text oldestText = oldest.GetComponent<TMP_Text>();
            oldestText.DOFade(0, fadeDuration).OnComplete(() =>
            {
                Destroy(oldest);
            });
            combatLogMessages.RemoveAt(0);
        }
        yield return new WaitForSeconds(delay);
        foreach (GameObject msg in combatLogMessages)
        {
            TMP_Text msgT = msg.GetComponent<TMP_Text>();
            msgT.DOFade(0, fadeDuration).OnComplete(() =>
            {
                Destroy(msg);
            });
        }
        combatLogMessages.Clear();
        yield return null;
    }

    #endregion

    #region Target Selection

    /// <summary>
    /// Starts the target selection mode for attacks or specials.
    /// </summary>
    public IEnumerator EnterTargetSelection(CardInstance card, bool attack)
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

    /// <summary>
    /// Exits the target selection mode and resets related state.
    /// </summary>
    public void ExitTargetSelection()
    {
        targetSelectionMode = false;
        selectedTarget = null;
        attackSelected = false;
        specialSelected = false;
        HideCardOptions();
    }

    #endregion

    #region Card Actions (Attack/Special)

    /// <summary>
    /// Called when the attack button is selected in the card options UI.
    /// </summary>
    public void SelectAttack(CardInstance card)
    {
        attackSelected = true;
        specialSelected = false;
        HideCardOptions();
        if (OccupiedEnemyPositions == 0)
        {
            card.Attack(card.owner, null);
            return;
        }
        StartCoroutine(EnterTargetSelection(selectedCard, true));
    }

    /// <summary>
    /// Called when the special button is selected in the card options UI.
    /// </summary>
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

    #endregion

    #region Game State & Utility

    private void Update()
    {
        if (targetSelectionMode && Input.GetMouseButtonUp(1))
        {
            ExitTargetSelection();
        }
    }

    public void AddToAvailableCards(CardInstance card)
    {
        playerCards.Add(card.card);
    }

    /// <summary>
    /// Handles game over logic.
    /// </summary>
    /// <param name="isPlayer">True if the player lost, false if the enemy lost.</param>
    public void GameOver(bool isPlayer)
    {
        if (isPlayer)
        {
            Debug.Log("Game Over. Player has lost.");
        }
        else
        {
            winCanvas.SetActive(true);
            UICanvas.SetActive(false);
            Debug.Log("Game Over. Player has won.");
        }
    }

    #endregion
}
