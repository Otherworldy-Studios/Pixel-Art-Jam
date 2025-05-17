using UnityEngine;

[CreateAssetMenu(fileName = "RadiatingZombie", menuName = "Undead Cards/RadiatingZombie")]
public class RadiatingZombie : UndeadCard
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
        throw new System.NotImplementedException();
    }

    public override void OnTurnEnd(PlayerStats owner, CardInstance instanceOwner)
    {
        throw new System.NotImplementedException();
    }

    public override void OnTurnStart(PlayerStats owner, CardInstance instanceOwner)
    {
        Debug.Log("Radiating Zombie special activated");
        if (owner.isPlayer)
        {
            if (GameManager.Instance.OccupiedEnemyPositions == 0)
            {
                TurnManager.Instance.enemy.TakeDamage(1);
            }
        }
        else
        {
            if (GameManager.Instance.OccupiedPlayerPositions == 0)
            {
                TurnManager.Instance.player.TakeDamage(1);
            }
        }

        for (int i = 0; i < GameManager.Instance.board.Count; i++)
        {
            if (GameManager.Instance.board[i].owner == owner)
            {
                continue;
            }
            GameManager.Instance.board[i].TakeDamage(1);
          
        }

       
    }
}
