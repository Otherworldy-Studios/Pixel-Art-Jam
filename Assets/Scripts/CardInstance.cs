using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

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
    Canvas canvas;
    [SerializeField] Canvas displayCanvas;
    public PlayerStats owner;
    public bool faceUp => currentPosition == CardPosition.PlayerHand || currentPosition == CardPosition.Board;

    [SerializeField] Vector3 originalRot;
    [SerializeField] Vector3 originalPos;
    public Transform originalParent;

    [SerializeField] Vector3 targetViewRot;
    [SerializeField] Vector3 targetViewScale;
    [SerializeField] Vector3 positionChange;

    public RectTransform rectTransform;




    private void Start()
    {
        originalRot = transform.localRotation.eulerAngles;
        originalPos = transform.localPosition;
        canvas = GetComponentInParent<Canvas>();
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
        healthText.text = card.currentHealth.ToString();

    }

    public void Update()
    {
        FaceUp(faceUp);
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

    public void DoSpecial()
    {
        card.DoSpecial();
    }
    public void DoSpecial(GameObject Owner, int manaCost)
    {
        card.DoSpecial(Owner, manaCost);

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
        if (currentPosition == CardPosition.EnemyHand) return;
        if(currentPosition == CardPosition.PlayerHand)
        {
            transform.DOBlendableLocalMoveBy(positionChange, 0.2f);
        }
        transform.DOLocalRotate(targetViewRot, 0.2f);
        transform.DOScale(targetViewScale, 0.2f);
        transform.parent = displayCanvas.transform;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentPosition == CardPosition.EnemyHand) return;
        if (currentPosition == CardPosition.PlayerHand)
        {
            transform.DOLocalRotate(originalRot, 0.2f);
            transform.DOBlendableLocalMoveBy(-positionChange, 0.2f);
        }
        transform.DOScale(Vector3.one, 0.2f);
        transform.parent = originalParent;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(currentPosition == CardPosition.EnemyHand) return;
        PlayCard();
    }


    public void Attack(GameObject owner, GameObject target)
    {

    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    public void PlayCard()
    {
        GameManager.Instance.PlayCard(this, isPlayerCard);
    }

}
