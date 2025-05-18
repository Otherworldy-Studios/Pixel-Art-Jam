using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Sirenix;
using Sirenix.OdinInspector;
using Sirenix.Serialization;


public abstract class CardSO : SerializedScriptableObject
{
    
    public Sprite icon;
    public int atk;
    public int currentHealth;
    public int maxHealth;
    public bool hasSpecialCondition;
    public SpecialCondition[] specialConditions;
    public string cardName;
    [TextArea(3, 10)]
    public string cardDescription;
    [TextArea(3, 10)]
    public string specialMessageText;
    public int manaCost;
    public bool canSelectTarget;
    public bool canTargetAllies;
    public bool canOnlyTargetAllies;
    public int lingeringDamage;
    public Sprite[] cardEffect;
    

    public abstract bool DoSpecial(PlayerStats Owner, CardInstance target, CardInstance instanceOwner);

    public abstract void OnDeath(PlayerStats owner, CardInstance instanceOwner, CardInstance deadCard);

    public abstract void OnTurnStart(PlayerStats owner, CardInstance instanceOwner);

    public abstract void OnTurnEnd(PlayerStats owner, CardInstance instanceOwner);

    public abstract void OnCardPlayed(PlayerStats owner, CardInstance cardPlayed, CardInstance instanceOwner);

    public abstract void OnCardDiscarded(PlayerStats owner, CardInstance cardPlayed);

    public abstract void OnDamageTaken(PlayerStats owner, CardInstance damaged, CardInstance instanceOwner, int amount);



}

public enum CardPosition
{
    Deck,
    PlayerHand,
    EnemyHand,
    Board,
    Discard
}

public enum SpecialConditions
{
    Death,
    DamageTaken,
    Kill,
    TurnStart,
    TurnEnd,
    Played,
}

public enum StatusEffects
{
    LingeringDamage,
    Slow,
    Paralyze,
    Sap,
    Invulnerable,
    Bolstered,
}

[System.Serializable]
public struct SpecialCondition
{
    public SpecialConditions condition;
    public bool affectsOtherCards;
   // public bool turnStart;
   // public int duration;
}
