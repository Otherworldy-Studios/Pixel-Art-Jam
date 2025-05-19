using UnityEngine;

[CreateAssetMenu(fileName = "UndeadPriest", menuName = "Undead Cards/UndeadPriest")]
public class UndeadPriest : UndeadCard
{
    public int healAmount = 4;
    public override bool DoSpecial(PlayerStats Owner, CardInstance target, CardInstance instanceOwner)
    {
        if (Owner.isPlayer)
        {
            if (GameManager.Instance.OccupiedPlayerPositions == 0)
            {
                return false;
            }
        }
        else
        {
            if (GameManager.Instance.OccupiedEnemyPositions == 0)
            {
                return false;
            }
        }
        specialMessageText = "Undead Priest heals " + target.cardName + " for " + healAmount.ToString() + " health.";
        instanceOwner.StartCoroutine(instanceOwner.PlayEffect(cardEffect, target.gameObject));
        target.Heal(healAmount);
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
