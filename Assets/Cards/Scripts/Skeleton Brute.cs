using UnityEngine;
[CreateAssetMenu(fileName = "SkeletonBrute", menuName = "Undead Cards/SkeletonBrute")]

public class SkeletonBrute : UndeadCard
{
    public override bool DoSpecial(PlayerStats Owner, CardInstance target, CardInstance instanceOwner)
    {
        if(target == null)
        {
            return false;
        }
        if(Owner == null)
        {
            return false;
        }
        if (Owner.isPlayer)
        {
            if(GameManager.Instance.OccupiedEnemyPositions == 0)
            {
               instanceOwner.Attack(Owner, null);
                return true;
            }
        }
        else
        {
            if (GameManager.Instance.OccupiedPlayerPositions == 0)
            {
                instanceOwner.Attack(Owner, null);
                return true;
            }
        }
        instanceOwner.TakeDamage(4);
        specialMessageText = $"{cardName} attacks recklessly";
        instanceOwner.Attack(Owner, target);
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
