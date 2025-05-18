using UnityEngine;

[CreateAssetMenu(fileName = "BasicGhost", menuName = "Undead Cards/BasicGhost")]
public class BasicGhost : UndeadCard
{
    public int specialDamage;
    public override bool DoSpecial(PlayerStats Owner, CardInstance target, CardInstance instanceOwner)
    {
       if(Owner.isPlayer)
       {
            TurnManager.Instance.enemy.TakeDamage(specialDamage);
            specialMessageText = $"Ghost deals direct damage to enemy";
        }
       else
       {
           TurnManager.Instance.player.TakeDamage(specialDamage);
           specialMessageText = $"Ghost deals direct damage to player";
       }
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
