using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArmoredZombie", menuName = "Undead Cards/ArmoredZombie")]
public class ArmoredZombie : UndeadCard
{
    public int damageAbsorbed = 3;
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
        HashSet<CardInstance> cards = new HashSet<CardInstance>();

        for (int i = 0; i < GameManager.Instance.board.Count; i++)
        {
            if (GameManager.Instance.board[i].owner == owner && GameManager.Instance.board[i].card is not ArmoredZombie)
            {

                if (GameManager.Instance.board[i].modifiedByArmoredZombie) { continue; }
                GameManager.Instance.board[i].damageModifier -= 3;
                GameManager.Instance.board[i].modifiedByArmoredZombie = true;

            }
        }



    }

    public override void OnDamageTaken(PlayerStats owner, CardInstance damaged, CardInstance instanceOwner, int amount)
    {
        if (damaged.owner == owner && damaged != instanceOwner && damaged.card is not ArmoredZombie)
        {
            instanceOwner.TakeDamage(damageAbsorbed);
        }
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
