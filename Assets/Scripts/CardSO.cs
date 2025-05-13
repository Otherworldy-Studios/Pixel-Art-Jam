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
    public string cardDescription;
    
   

    public abstract void DoSpecial();
    public abstract void DoSpecial(GameObject Owner, int manaCost);
    public abstract void Attack(GameObject owner, GameObject target);
    public abstract void TakeDamage(int damage);

   
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

