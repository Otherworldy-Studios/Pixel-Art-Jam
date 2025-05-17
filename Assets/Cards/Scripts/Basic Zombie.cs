using UnityEngine;
[CreateAssetMenu(fileName = "BasicZombie", menuName = "Undead Cards/BasicZombie")]


public class BasicZombie : UndeadCard
{
    public override bool DoSpecial(PlayerStats Owner, CardInstance target, CardInstance instanceOwner)
    {
        return false;
    }

    public override void OnCardDiscarded(PlayerStats owner, CardInstance cardPlayed)
    {
        
    }

    public override void OnCardPlayed(PlayerStats owner, CardInstance cardPlayed, CardInstance instanceOwner)
    {
        
    }

    public override void OnDamageTaken(PlayerStats owner, CardInstance damaged, CardInstance instanceOwner, int amount)
    {
        
    }

    public override void OnDeath(PlayerStats owner, CardInstance instanceOwner, CardInstance deadCard)
    {
        
    }

    public override void OnTurnEnd(PlayerStats owner, CardInstance instanceOwner)
    {
        
    }

    public override void OnTurnStart(PlayerStats owner, CardInstance instanceOwner)
    {
        
    }
}
