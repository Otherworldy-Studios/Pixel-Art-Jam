using UnityEngine;

[CreateAssetMenu(fileName = "RelentlessZombie", menuName = "Undead Cards/RelentlessZombie")]
public class RelentlessZombie : UndeadCard
{
    public int amountToHeal = 3;
    public override bool DoSpecial(PlayerStats Owner, CardInstance target, CardInstance instanceOwner)
    {
        if (!Owner.isPlayer && GameManager.Instance.OccupiedPlayerPositions == 0)
        {
            return false;
        }
        if (Owner.isPlayer && GameManager.Instance.OccupiedEnemyPositions == 0)
        {
            return false;
        }
        if (target == null)
        {
            return false;
        }
        if (Owner == null)
        {
            return false;
        }
        if(instanceOwner == null)
        {
            return false;
        }
       target.TakeDamage(amountToHeal);
       instanceOwner.Heal(amountToHeal);
        instanceOwner.StartCoroutine(instanceOwner.PlayEffect(cardEffect, target.gameObject));
        specialMessageText = $"{cardName} drains {amountToHeal} health from {target.card.cardName}";
        return true;
    }

    public override void OnCardDiscarded(PlayerStats owner, CardInstance cardPlayed)
    {
        throw new System.NotImplementedException();
    }

    public override void OnCardPlayed(PlayerStats owner, CardInstance cardPlayed, CardInstance instanceOwner)
    {
        throw new System.NotImplementedException();
    }

    public override void OnDamageTaken(PlayerStats owner, CardInstance damaged, CardInstance instanceOwner, int amount)
    {
        throw new System.NotImplementedException();
    }

    public override void OnDeath(PlayerStats owner, CardInstance instanceOwner, CardInstance deadCard)
    {
        throw new System.NotImplementedException();
    }

    public override void OnTurnEnd(PlayerStats owner, CardInstance instanceOwner)
    {
        throw new System.NotImplementedException();
    }

    public override void OnTurnStart(PlayerStats owner, CardInstance instanceOwner)
    {
        throw new System.NotImplementedException();
    }
}
