using UnityEngine;

[CreateAssetMenu(fileName = "Banshee", menuName = "Undead Cards/Banshee")]
public class Banshee : UndeadCard
{
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
        target.ApplyStatusEffect(StatusEffects.Paralyze, 3);
        specialMessageText = $"{cardName} paralyzes {target.card.cardName} for 3 turns";
        instanceOwner.StartCoroutine(instanceOwner.PlayEffect(cardEffect));
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
