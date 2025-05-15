using UnityEngine;
using UnityEngine.Analytics;

[CreateAssetMenu(fileName = "SkeletonKnight", menuName = "Undead Cards/SkeletonKnight")]
public class SkeletonKnight : UndeadCard
{
    public override bool DoSpecial(PlayerStats Owner, CardInstance target, CardInstance instanceOwner)
    {
        if(GameManager.Instance.OccupiedEnemyPositions == 0)
        {
            Debug.Log("Skeleton Knight special failed");
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
      
        return true;
    }
}
