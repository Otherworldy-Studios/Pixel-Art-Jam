using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public List<CardInstance> board;
    public Transform[] playerBoardSide;
    public Transform[] enemyBoardSide;
    public Transform playerEnvironment;
    public Transform enemyEnvironment;

    public int OccupiedPlayerPositions = 0;
    public int OccupiedEnemyPositions = 0;

    public void PlayCard(CardInstance card, bool isPlayer)
    {
        board.Add(card);
        card.currentPosition = CardPosition.Board;
        if(isPlayer)
        {
            if(OccupiedPlayerPositions >= playerBoardSide.Length)
            {
                Debug.LogError("No more positions available on the player board");
                return;
            }
            card.gameObject.transform.parent = playerBoardSide[OccupiedPlayerPositions];
            card.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
            card.originalParent = playerBoardSide[OccupiedPlayerPositions];
            OccupiedPlayerPositions++;
        }
        else
        {
            if(OccupiedEnemyPositions >= enemyBoardSide.Length)
            {
                Debug.LogError("No more positions available on the enemy board");
                return;
            }
            card.gameObject.transform.parent = enemyBoardSide[OccupiedEnemyPositions];
            card.transform.DOLocalRotate(new Vector3(0, 180, 0), 0.5f);
            card.originalParent = enemyBoardSide[OccupiedEnemyPositions];
            OccupiedEnemyPositions++;
        }

        card.rectTransform.anchoredPosition = Vector3.zero;
       
    }

}


