using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;
using Sirenix.OdinInspector;
using System;
using JetBrains.Annotations;


/// <summary>
/// Represents a single card instance in the game, handling its state, effects, and interactions.
/// </summary>
public class CardInstance : SerializedMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    #region Fields and Properties

    // --- Card Data ---
    [Header("Card Data")]
    public Sprite cardFaceUp;
    public Sprite cardFaceDown;
    public int atk;
    public int currentHealth;
    public int maxHealth;
    public CardPosition currentPosition;
    public Dictionary<StatusEffects, int> statusEffects = new();
    public CardSO card;
    public int damageModifier = 0;
    public bool modifiedByArmoredZombie = false;

    // --- UI References ---
    [Header("UI References")]
    public Image cardImage;
    public Image iconImage;
    public TMP_Text cardName;
    public TMP_Text cardDescription;
    public TMP_Text atkText;
    public TMP_Text healthText;
    public Canvas displayCanvas;
    public WinCanvas winCanvas;

    // --- Ownership & State ---
    [Header("Ownership & State")]
    public PlayerStats owner;
    public bool isPlayerCard;
    public bool faceUp => currentPosition == CardPosition.PlayerHand || currentPosition == CardPosition.Board || currentPosition == CardPosition.Store || currentPosition == CardPosition.CanAddToDeck || currentPosition == CardPosition.StoreDeck;

    // --- Transform & Animation ---
    [Header("Transform & Animation")]
    [SerializeField] Vector3 originalRot;
    public Transform originalParent;
    [SerializeField] Vector3 targetViewRot;
    [SerializeField] Vector3 targetViewScale;
    [SerializeField] Vector3 positionChange;
    public RectTransform rectTransform;
    public Image cardEffect;

    // --- Flags ---
    [Header("Flags")]
    public bool moving = false;
    public bool hasDoneSpecial = false;
    public bool hasDoneAttack = false;
    public bool slowed;
    public bool isParalyzed;
    public bool isPossessing = false;
    public bool bolstered = false;

    // --- Internal State ---
    private Color originalColor = Color.white;
    private bool selectable = true;

    #endregion

    #region Events

    /// <summary>
    /// Invoked when this card dies.
    /// </summary>
    public event Action<CardInstance> OnCardDeath;

    /// <summary>
    /// Invoked when this card takes damage.
    /// </summary>
    public event Action<CardInstance, int> OnCardDamage;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        originalRot = transform.localRotation.eulerAngles;
        originalParent = transform.parent;
        rectTransform = GetComponent<RectTransform>();
        displayCanvas = GameObject.Find("DisplayCanvas").GetComponent<Canvas>();
        if(currentPosition == CardPosition.Store || currentPosition == CardPosition.StoreDeck || currentPosition == CardPosition.CanAddToDeck )
        {
            winCanvas = GetComponentInParent<WinCanvas>();
        }
           
       
    }

    private void Update()
    {
        FaceUp(faceUp);
        CheckPositionForInteraction();
        DoPassive();
        healthText.text = currentHealth.ToString();
        atkText.text = atk.ToString();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes this card instance with the given card data.
    /// </summary>
    /// <param name="card">The card data (CardSO) to initialize from.</param>
    public void Initialize(CardSO card)
    {
        atk = card.atk;
        maxHealth = card.maxHealth;
        currentHealth = card.maxHealth;
        iconImage.sprite = card.icon;
        this.card = card;
        this.cardName.text = card.cardName;
        cardDescription.text = card.cardDescription;
        atkText.text = card.atk.ToString();
        healthText.text = currentHealth.ToString();
    }

    #endregion

    #region UI and Interaction

    /// <summary>
    /// Checks if the card should be interactable based on its position.
    /// </summary>
    public void CheckPositionForInteraction()
    {
        if (currentPosition == CardPosition.Deck)
            SetInteractable(false);
        else
            SetInteractable(true);
    }

    /// <summary>
    /// Sets whether the card is interactable in the UI.
    /// </summary>
    public void SetInteractable(bool isInteractable)
    {
        cardImage.raycastTarget = isInteractable;
    }

    /// <summary>
    /// Sets the card face up or down visually.
    /// </summary>
    public void FaceUp(bool shouldFaceUp)
    {
        if (shouldFaceUp)
        {
            cardImage.sprite = cardFaceUp;
            iconImage.enabled = true;
            cardName.enabled = true;
            cardDescription.enabled = true;
            atkText.enabled = true;
            healthText.enabled = true;
        }
        else
        {
            cardImage.sprite = cardFaceDown;
            iconImage.enabled = false;
            cardName.enabled = false;
            cardDescription.enabled = false;
            atkText.enabled = false;
            healthText.enabled = false;
            cardEffect.enabled = false;
        }
    }

    /// <summary>
    /// Shakes the mana UI to indicate an error (e.g., not enough mana).
    /// </summary>
    public void ShakeManaUI()
    {
        if (owner.manaText == null)
            return;
        owner.manaText.gameObject.transform.DOShakeRotation(0.2f, new Vector3(0, 0, 10), 10, 10);
    }

    #endregion

    #region Pointer Event Handlers

    /// <summary>
    /// Handles pointer enter events for hover effects.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (moving || !selectable || currentPosition == CardPosition.EnemyHand ||
            currentPosition == CardPosition.Deck || currentPosition == CardPosition.Discard)
            return;

        if (currentPosition == CardPosition.Board)
        {
            //   Debug.Log($"Hovered over {card.cardName}");
        }

        if (currentPosition == CardPosition.PlayerHand)
        {
            transform.DOBlendableLocalMoveBy(positionChange, 0.2f);
        }

        if(currentPosition == CardPosition.Store)
        {
            winCanvas.CardHover(true);
        }

        transform.DOLocalRotate(targetViewRot, 0.2f);
        transform.DOScale(targetViewScale, 0.2f);
        transform.SetParent(displayCanvas.transform);
    }

    /// <summary>
    /// Handles pointer exit events for hover effects.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (moving || !selectable || currentPosition == CardPosition.EnemyHand ||
            currentPosition == CardPosition.Deck || currentPosition == CardPosition.Discard)
            return;

        if (currentPosition == CardPosition.PlayerHand)
            transform.DOBlendableLocalMoveBy(-positionChange, 0.2f);

        if (currentPosition == CardPosition.Store)
        {
            winCanvas.CardHover(true);
        }


        transform.DOLocalRotate(originalRot, 0.2f);
        transform.DOScale(Vector3.one, 0.2f);
        transform.SetParent(originalParent);
    }

    /// <summary>
    /// Handles pointer down events for card selection and play.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (moving || !selectable || currentPosition == CardPosition.EnemyHand ||
            currentPosition == CardPosition.Deck || currentPosition == CardPosition.Discard)
            return;

        if (currentPosition == CardPosition.Board && !GameManager.Instance.targetSelectionMode)
        {
            if (owner.isPlayer)
            {
                if (eventData.button == PointerEventData.InputButton.Right && owner.isMyTurn)
                {
                    GameManager.Instance.DiscardCard(this, isPlayerCard);
                    return;
                }
                if (!owner.isMyTurn || hasDoneAttack || hasDoneSpecial || isParalyzed)
                    return;
                GameManager.Instance.selectedCard = this;
                GameManager.Instance.ShowCardOptions(this);
            }
        }
        else if (currentPosition == CardPosition.Board)
        {
            if (GameManager.Instance.targetSelectionMode)
            {
                if (GameManager.Instance.specialSelected && !GameManager.Instance.selectedCard.card.canSelectTarget)
                {
                    return;
                }
                if (owner.isPlayer && GameManager.Instance.selectedCard.card.canTargetAllies)
                {
                    GameManager.Instance.selectedTarget = this;
                }
                else if (!owner.isPlayer)
                {
                    GameManager.Instance.selectedTarget = this;
                }
                else
                {
                    Debug.Log("Invalid target selection");
                    return;
                }
            }

        }
        if (currentPosition == CardPosition.PlayerHand)
        {
            if (!owner.isMyTurn)
                return;
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                GameManager.Instance.DiscardCard(this, isPlayerCard);
                return;
            }
            PlayCard();
        }

        if(currentPosition == CardPosition.CanAddToDeck)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if(GameManager.Instance.playerCards.Contains(card))
                {
                    GameManager.Instance.playerCards.Remove(card);
                    SceneCounter.Instance.PlayerCards.Remove(card);
                    winCanvas.UpdateCards();
                }
                return;
            }
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (!GameManager.Instance.playerCards.Contains(card))
                {
                    GameManager.Instance.playerCards.Add(card);
                    SceneCounter.Instance.PlayerCards.Add(card);
                    winCanvas.UpdateCards();
                }
                return;
            }
        }

        if(currentPosition == CardPosition.Store)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (!GameManager.Instance.playerCards.Contains(card))
                {
                    GameManager.Instance.playerCards.Add(card);
                    SceneCounter.Instance.PlayerCards.Add(card);
                    GameManager.Instance.allCards.Add(card);
                    winCanvas.cardsGained.Remove(this);
                    winCanvas.allCardsInCanvas.Remove(this);
                    winCanvas.UpdateCards();
                    Destroy(gameObject);
                }
                return;
            }
        }

        if (currentPosition == CardPosition.StoreDeck)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (!GameManager.Instance.playerCards.Contains(card))
                {
                    GameManager.Instance.playerCards.Remove(card);
                    SceneCounter.Instance.PlayerCards.Remove(card);
                    winCanvas.currentDeck.Remove(this);
                    winCanvas.allCardsInCanvas.Remove(this);
                    winCanvas.UpdateCards();
                    Destroy(this.gameObject);
                }
                return;
            }
        }
    }

    #endregion

    #region Status Effects

    /// <summary>
    /// Applies a status effect to this card.
    /// </summary>
    public void ApplyStatusEffect(StatusEffects effect, int duration)
    {
        if (!statusEffects.ContainsKey(effect))
        {
            statusEffects.Add(effect, duration);
        }


        if (effect == StatusEffects.Slow)
        {
            slowed = true;
        }
        if (effect == StatusEffects.Paralyze)
        {
            isParalyzed = true;
        }
        if (effect == StatusEffects.Bolstered)
        {
            bolstered = true;
            atk += 3;
        }
    }

    /// <summary>
    /// Decrements the duration of all status effects and removes expired ones.
    /// </summary>
    public void DecrementStatusEffects()
    {
        List<StatusEffects> effectsToRemove = new List<StatusEffects>();
        // Iterate over a copy of the keys to avoid modifying the collection during enumeration
        foreach (var key in new List<StatusEffects>(statusEffects.Keys))
        {
            statusEffects[key]--;
            if (statusEffects[key] <= 0)
                effectsToRemove.Add(key);
        }

        foreach (var effect in effectsToRemove)
        {
            statusEffects.Remove(effect);
            if (effect == StatusEffects.Slow)
            {
                slowed = false;
            }
            if (effect == StatusEffects.Paralyze)
            {
                isParalyzed = false;
            }
            if (effect == StatusEffects.Bolstered)
            {
                bolstered = false;
                atk = card.atk;
            }

        }
    }

    /// <summary>
    /// Applies the effects of all current status effects.
    /// </summary>
    public void DoStatusEffects()
    {
        if (statusEffects == null || statusEffects.Count == 0)
            return;

        foreach (KeyValuePair<StatusEffects, int> effect in statusEffects)
        {
            switch (effect.Key)
            {
                case StatusEffects.LingeringDamage:
                    TakeDamage(card.lingeringDamage);
                    break;
                case StatusEffects.Slow:
                    slowed = true;
                    break;
                case StatusEffects.Paralyze:
                    isParalyzed = true;
                    break;
                case StatusEffects.Sap:
                    atk -= 3;
                    break;
                default:
                    break;
            }
        }
        DecrementStatusEffects();
    }

    #endregion

    #region Card Events and Turn Logic



    /// <summary>
    /// Subscribes to card-specific events (e.g., death).
    /// </summary>
    public void SubscribeToCardEvents()
    {
        if (card.specialConditions.Length > 0)
        {
            foreach (SpecialCondition specialCondition in card.specialConditions)
            {
                if (card is UndeadCard && specialCondition.condition == SpecialConditions.Death)
                {
                    if (specialCondition.affectsOtherCards)
                    {
                        foreach (CardInstance cardOnBoard in GameManager.Instance.board)
                        {
                            if (cardOnBoard.card is UndeadCard)
                            {
                                OnCardDeath += OnDeath;
                            }
                        }
                    }
                    OnCardDeath += OnDeath;
                }
                if (specialCondition.condition == SpecialConditions.TurnStart)
                {
                    TurnManager.Instance.OnTurnStart += OnTurnStart;
                    if (specialCondition.affectsOtherCards)
                    {
                        foreach (CardInstance cardOnBoard in GameManager.Instance.board)
                        {
                            TurnManager.Instance.OnTurnStart += OnTurnStart;
                        }
                    }
                }
                if (specialCondition.condition == SpecialConditions.DamageTaken)
                {
                    OnCardDamage += OnDamageTaken;
                    if (specialCondition.affectsOtherCards)
                    {
                        foreach (CardInstance cardOnBoard in GameManager.Instance.board)
                        {
                            OnCardDamage += OnDamageTaken;
                        }
                    }
                }
                if (specialCondition.condition == SpecialConditions.Played)
                {
                    GameManager.Instance.OnCardPlayed += OnCardPlayed;
                    if (specialCondition.affectsOtherCards)
                    {
                        foreach (CardInstance cardOnBoard in GameManager.Instance.board)
                        {
                            GameManager.Instance.OnCardPlayed += OnCardPlayed;
                        }
                    }
                }
            }
        }
    }


    public void UnsubscribeToCardEvents()
    {
        if (card.specialConditions != null && card.specialConditions.Length > 0)
        {
            foreach (SpecialCondition specialCondition in card.specialConditions)
            {
                if (card is UndeadCard && specialCondition.condition == SpecialConditions.Death)
                {
                    OnCardDeath -= OnDeath;
                }
                if (specialCondition.condition == SpecialConditions.TurnStart)
                {
                    TurnManager.Instance.OnTurnStart -= OnTurnStart;
                }
                if (specialCondition.condition == SpecialConditions.DamageTaken)
                {
                    OnCardDamage -= OnDamageTaken;
                }
                if (specialCondition.condition == SpecialConditions.Played)
                {
                    GameManager.Instance.OnCardPlayed -= OnCardPlayed;
                }
            }
        }
    }

    /// <summary>
    /// Called when this card dies.
    /// </summary>
    public void OnDeath(CardInstance deadCard)
    {
        card.OnDeath(owner, this, deadCard);
    }
    public void OnTurnStart()
    {
        card.OnTurnStart(owner, this);
    }
    public void OnDamageTaken(CardInstance damaged, int amount)
    {
        card.OnDamageTaken(owner, damaged, this, amount);
    }
    private void OnCardPlayed(CardInstance instance)
    {
        card.OnCardPlayed(owner, instance, this);
    }
    #endregion

    #region Card Actions

    /// <summary>
    /// Attempts to perform the card's special ability.
    /// </summary>
    /// <param name="Owner">The owner of the card.</param>
    /// <param name="target">The target card instance.</param>
    /// <returns>True if the special was performed, false otherwise.</returns>
    public bool DoSpecial(PlayerStats Owner, CardInstance target)
    {
        selectable = false;
        if (owner.currentMana < card.manaCost)
        {
            Debug.Log("Not enough mana");
            ShakeManaUI();
            return false;
        }
        if (slowed)
        {
            Debug.Log("Card is slowed");
            return false;
        }
        if (isParalyzed)
        {
            Debug.Log("Card is paralyzed");
            return false;
        }
        if (target == null && card.canSelectTarget)
        {
            Debug.LogError("Target is null");
            return false;
        }
        if (Owner == null)
        {
            Debug.LogError("Owner is null");
            return false;
        }
        GameManager.Instance.EnqueueActionMessage(card.specialMessageText);
        GameManager.Instance.specialSelected = false;
        selectable = true;
        hasDoneSpecial = card.DoSpecial(Owner, target, this);


        if (hasDoneSpecial)
            owner.currentMana -= card.manaCost;

        return hasDoneSpecial;
    }



    /// <summary>
    /// Applies the card's passive effect if it is an environment card.
    /// </summary>
    public void DoPassive()
    {
        if (card is EnvironmentCard envCard)
        {
            while (currentPosition == CardPosition.Board)
            {
                envCard.PassiveEffect(owner.gameObject);
            }
        }
    }

    /// <summary>
    /// Performs an attack on the target card.
    /// </summary>
    /// <param name="owner">The owner of the attacking card.</param>
    /// <param name="target">The target card instance.</param>
    /// <param name="modifier">Damage modifier.</param>
    public void Attack(PlayerStats owner, CardInstance target, int modifier = 0)
    {
        if (owner.currentMana < 2)
        {
            Debug.Log("Not enough mana");
            ShakeManaUI();
            return;
        }

        selectable = false;
        int finalDamage = atk + modifier;
        if (target != null)
        {
            GameManager.Instance.EnqueueActionMessage($"{card.cardName} attacks {target.card.cardName} for {finalDamage} damage");
        }
        if (target == null)
        {
            if (owner.isPlayer)
            {
                if (GameManager.Instance.OccupiedEnemyPositions <= 0)
                {
                    TurnManager.Instance.enemy.TakeDamage(finalDamage);
                    GameManager.Instance.EnqueueActionMessage($"Player attacks enemy for {finalDamage} damage");
                }

            }
            else
            {
                if (GameManager.Instance.OccupiedPlayerPositions <= 0)
                {
                    TurnManager.Instance.player.TakeDamage(finalDamage);
                    GameManager.Instance.EnqueueActionMessage($"Enemy attacks player for {finalDamage} damage");
                }
            }

        }
        if (card is UndeadCard cardSO)
        {
            if (target != null)
            {
                target.TakeDamage(finalDamage);
                transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 1, 1);
            }
        }
        owner.currentMana -= 2;
        GameManager.Instance.attackSelected = false;
        hasDoneAttack = true;
        selectable = true;
    }

    /// <summary>
    /// Resets the attack and special flags for this card.
    /// </summary>
    public void ResetFlags()
    {
        hasDoneAttack = false;
        hasDoneSpecial = false;
    }

    /// <summary>
    /// Applies damage to this card and handles death or damage feedback.
    /// </summary>
    /// <param name="damage">Amount of damage to apply.</param>
    public void TakeDamage(int damage)
    {
        selectable = false;
        damage += damageModifier;
        if (damage > currentHealth)
        {
            int overkill = damage - currentHealth;
            owner.TakeDamage(overkill);
        }
        currentHealth -= damage;
        OnCardDamage?.Invoke(this, damage);
        if (currentHealth <= 0)
        {
            GameManager.Instance.DiscardCard(this, isPlayerCard);
            OnCardDeath?.Invoke(this);
        }

        StartCoroutine(CardFlash(Color.red));

        selectable = true;
    }

    public void Heal(int amount)
    {
        if (currentHealth + amount > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += amount;
        }
        StartCoroutine(CardFlash(Color.green));
    }

    public void ShakeCard()
    {
        transform.DOShakeRotation(0.2f, new Vector3(0, 0, 10), 10, 10);
    }



    /// <summary>
    /// Plays this card from hand to the board.
    /// </summary>
    public void PlayCard()
    {
        StartCoroutine(GameManager.Instance.PlayCard(this, isPlayerCard));
    }

    #endregion

    #region Visual Feedback

    /// <summary>
    /// Coroutine to briefly flash the card red when damaged.
    /// </summary>
    public IEnumerator CardFlash(Color color)
    {
        cardImage.color = color;
        yield return new WaitForSeconds(0.2f);
        cardImage.color = originalColor;
    }


    public IEnumerator PlayEffect(Sprite[] animationSprites)
    {
        if (animationSprites.Length == 0)
        {
            Debug.LogError("No animation sprites provided");
            yield break;
        }
        cardEffect.gameObject.SetActive(true);
        cardEffect.sprite = animationSprites[0];
        for (int i = 0; i < animationSprites.Length; i++)
        {
            cardEffect.sprite = animationSprites[i];
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("Effect finished");
        cardEffect.gameObject.SetActive(false);
    }

    public IEnumerator PlayEffect(Sprite[] animationSprites, GameObject target)
    {
        cardEffect.gameObject.SetActive(true);
        cardEffect.sprite = animationSprites[0];
        cardEffect.transform.DOMove(target.transform.position, 0.2f);
        for (int i = 0; i < animationSprites.Length; i++)
        {
            cardEffect.sprite = animationSprites[i];
            yield return new WaitForSeconds(0.1f);
        }
        cardEffect.gameObject.SetActive(false);
    }
    #endregion
}
