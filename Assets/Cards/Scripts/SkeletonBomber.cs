using UnityEngine;

[CreateAssetMenu(fileName = "SkeletonBomber", menuName = "Undead Cards/SkeletonBomber")]
public class SkeletonBomber : UndeadCard
{
    public override bool DoSpecial(PlayerStats Owner, CardInstance target, CardInstance instanceOwner)
    {
        return false;
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
        if(deadCard == instanceOwner)
        {
            for (int i = 0; i < GameManager.Instance.board.Count; i++)
            {
                if (GameManager.Instance.board[i].owner == owner)
                {
                    continue;
                }
                GameManager.Instance.board[i].TakeDamage(8);
            }
            Debug.Log("Skeleton Bomber special triggered");
            specialMessageText = $"{cardName} explodes dealing 8 damage to all enemies";
            GameManager.Instance.EnqueueActionMessage(specialMessageText);
        }
      
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
