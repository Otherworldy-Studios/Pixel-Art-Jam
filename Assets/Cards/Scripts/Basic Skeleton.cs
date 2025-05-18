
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BasicSkeleton", menuName = "Undead Cards/BasicSkeleton")]
public class BasicSkeleton : UndeadCard
{
    public GameObject cardPrefab;
    public UndeadCard skeletonBrute;
    public UndeadCard skeletonKnight;

    public override bool DoSpecial(PlayerStats Owner,CardInstance instanceOwner = null, CardInstance target = null)
    {
        int skeletonsOnBoard = 0;
        if (!Owner.isPlayer)
        {
            return false;
        }
        foreach (CardInstance card in GameManager.Instance.board)
        {
            if (card.card is BasicSkeleton && card.owner == Owner)
            {
                skeletonsOnBoard++;
            }
        }

        
        if (skeletonsOnBoard > 1)
        {
            CardInstance cardGenerated = null;
            List<CardInstance> skeletons = new List<CardInstance>();
            for (int i = 0; i < GameManager.Instance.board.Count; i++)
            {
                if (GameManager.Instance.board[i].card is BasicSkeleton bs && GameManager.Instance.board[i].owner == Owner)
                {
                    skeletons.Add(GameManager.Instance.board[i]);
                }

            }
            foreach (CardInstance card in skeletons)
            {
                GameManager.Instance.DiscardCard(card, Owner.isPlayer);
            }
            if (skeletonsOnBoard == 2)
            {
                cardGenerated = GameManager.Instance.InstantiateCard(skeletonBrute, Owner, CardPosition.Board);

            }
            else if (skeletonsOnBoard == 3)
            {
                cardGenerated =  GameManager.Instance.InstantiateCard(skeletonKnight, Owner, CardPosition.Board);
            }
            GameManager.Instance.StartPlayCardCoroutine(cardGenerated, Owner.isPlayer, false);

        }
        else
        {
            Debug.Log("Not enough skeletons on the board");
            return false;
            //TODO card shake, show ui and play sound
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
