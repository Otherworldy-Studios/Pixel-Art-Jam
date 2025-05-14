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
                return false;
            }
        }
        else
        {
            if (GameManager.Instance.OccupiedPlayerPositions == 0)
            {
                return false;
            }
        }
       instanceOwner.TakeDamage(4);
        Debug.Log($"Skeleton Brute special activated attacking {target}");
        instanceOwner.Attack(Owner, target);
        return true;
    }
}
