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
    public string cardName;
    [TextArea(3, 10)]
    public string cardDescription;
    public int manaCost;
    public bool canSelectTarget;
    
    public abstract bool DoSpecial(PlayerStats Owner, CardInstance target, CardInstance instanceOwner);
  


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

}

public enum StatusEffects
{
    LingeringDamage,
    Slow,
    Paralyze,
    Sap
}

