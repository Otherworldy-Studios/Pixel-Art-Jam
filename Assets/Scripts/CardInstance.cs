using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

public class CardInstance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Sprite cardFaceUp;
    public Sprite cardFaceDown;
    public int atk;
    public int currentHealth;
    public int maxHealth;
    public CardPosition currentPosition;
    [OdinSerialize] public Dictionary<StatusEffects, int> statusEffects;
    public CardSO card;
    public Image cardImage;
    public Image iconImage;
    public bool isPlayerCard;
    public TMP_Text cardName;
    public TMP_Text cardDescription;
    public TMP_Text atkText;
    public TMP_Text healthText;

    public Canvas displayCanvas;
    public PlayerStats owner;
    public bool faceUp => currentPosition == CardPosition.PlayerHand || currentPosition == CardPosition.Board;

    [SerializeField] Vector3 originalRot;
    public Transform originalParent;

    [SerializeField] Vector3 targetViewRot;
    [SerializeField] Vector3 targetViewScale;
    [SerializeField] Vector3 positionChange;

    public RectTransform rectTransform;

    public bool moving = false;

    Color originalColor = Color.white;




    private void Awake()
    {
        originalRot = transform.localRotation.eulerAngles;
        originalParent = transform.parent;
        rectTransform = GetComponent<RectTransform>();
    }


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

    public void Update()
    {
        FaceUp(faceUp);
        CheckPositionForInteraction();
        DoPassive();
        healthText.text = currentHealth.ToString();
    }

    public void CheckPositionForInteraction()
    {
        if (currentPosition == CardPosition.Deck)
        {
            SetInteractable(false);
        }
        else
        {
            SetInteractable(true);
        }
    }

    public void SetInteractable(bool isInteractable)
    {
        cardImage.raycastTarget = isInteractable;
    }

    public void FaceUp(bool shouldFaceUp)
    {
        if (shouldFaceUp)
        {
            cardImage.sprite = cardFaceUp;
            iconImage.enabled = true;
            cardName.enabled = true;
            cardDescription.enabled = true;
            iconImage.enabled = true;
            atkText.enabled = true;
            healthText.enabled = true;

        }
        else
        {
            cardImage.sprite = cardFaceDown;
            iconImage.enabled = false;
            cardName.enabled = false;
            cardDescription.enabled = false;
            iconImage.enabled = false;
            atkText.enabled = false;
            healthText.enabled = false;
        }
    }


    public bool DoSpecial(PlayerStats Owner, CardInstance target)
    {
        if(target == null && card.canSelectTarget)
        {
           Debug.LogError("Target is null");
            return false;
        }
        if(Owner == null)
        {
            Debug.LogError("Owner is null");
            return false;
        }
        GameManager.Instance.specialSelected = false;
        return card.DoSpecial(Owner, target, this);

    }
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
    public void ApplyStatusEffect(StatusEffects effect, int duration)
    {
        statusEffects.Add(effect, duration);
    }


    public void DecrementStatusEffects()
    {
        List<StatusEffects> effectsToRemove = new List<StatusEffects>();
        foreach (var effect in statusEffects)
        {
            statusEffects[effect.Key]--;
            if (statusEffects[effect.Key] <= 0)
            {
                effectsToRemove.Add(effect.Key);
            }
        }

        foreach (var effect in effectsToRemove)
        {
            statusEffects.Remove(effect);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (moving) return;
        if (currentPosition == CardPosition.EnemyHand) return;
        if (currentPosition == CardPosition.Deck) return;
        if (currentPosition == CardPosition.Discard) return;
        if (currentPosition == CardPosition.Board )
        {
            Debug.Log($"Hovered over{card.cardName}");
        }
        if (currentPosition == CardPosition.PlayerHand)
        {
            transform.DOBlendableLocalMoveBy(positionChange, 0.2f);
        }
        transform.DOLocalRotate(targetViewRot, 0.2f);
        transform.DOScale(targetViewScale, 0.2f);
        transform.parent = displayCanvas.transform;
        

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (moving) return;
        if (currentPosition == CardPosition.EnemyHand) return;
        if (currentPosition == CardPosition.Deck) return;
        if (currentPosition == CardPosition.Discard) return;
        if (currentPosition == CardPosition.PlayerHand)
        {
           
            transform.DOBlendableLocalMoveBy(-positionChange, 0.2f);
        }
        transform.DOLocalRotate(originalRot, 0.2f);
        transform.DOScale(Vector3.one, 0.2f);
        transform.parent = originalParent;


    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (moving) return;
        if (currentPosition == CardPosition.EnemyHand) return;
        if (currentPosition == CardPosition.Deck) return;
        if (currentPosition == CardPosition.Discard) return;
        if (currentPosition == CardPosition.Board && owner.isPlayer)
        {
            //TODO should give option to do attack or do special but for now return 
            GameManager.Instance.selectedCard = this;
            GameManager.Instance.ShowCardOptions(this);

        }
        else if (currentPosition == CardPosition.Board && !owner.isPlayer)
        {
            if (GameManager.Instance.targetSelectionMode)
            {
                GameManager.Instance.selectedTarget = this;
            }
        }
        if (currentPosition == CardPosition.PlayerHand)
        {
            PlayCard();
        }


    }


    public void Attack(PlayerStats owner, CardInstance target, int modifier = 0)
    {
        int finalDamage = atk + modifier;
        if(target == null)
        {
            Debug.LogError("Target is null");
            return;
        }
        if (card is UndeadCard cardSO)
        {
            target.TakeDamage(finalDamage);
            transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 1, 1);
        }
        GameManager.Instance.attackSelected = false;

    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GameManager.Instance.DiscardCard(this, isPlayerCard);
        }
        else
        {
            transform.DOShakeRotation(0.2f, new Vector3(0, 0, 10), 10, 10);
            StartCoroutine(FlashRed());
        }
    }

    public void PlayCard()
    {
        GameManager.Instance.PlayCard(this, isPlayerCard);
    }

    private IEnumerator FlashRed()
    {
        cardImage.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        cardImage.color = originalColor;
        
    }

}
