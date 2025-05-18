using UnityEngine;
using UnityEngine.Analytics;

[CreateAssetMenu(fileName = "SkeletonKnight", menuName = "Undead Cards/SkeletonKnight")]
public class SkeletonKnight : UndeadCard
{
    public override bool DoSpecial(PlayerStats Owner, CardInstance target, CardInstance instanceOwner)
    {
        if(GameManager.Instance.OccupiedEnemyPositions == 0)
        {
            
            return false;
        }
        for (int i = 0; i < GameManager.Instance.board.Count; i++)
        {
            if (GameManager.Instance.board[i].owner == Owner)
            { 
                continue;
            }
            GameManager.Instance.board[i].TakeDamage(4);
        }
        specialMessageText = $"{cardName} does a sweep attack";
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
