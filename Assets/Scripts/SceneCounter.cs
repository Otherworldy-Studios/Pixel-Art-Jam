using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCounter : Singleton<SceneCounter>
{
    public int sceneCount = 1;
    public List<CardSO> Enemy1Cards;
    public List<CardSO> Enemy2Cards;
    public List<CardSO> Enemy3Cards;
    public List<CardSO> Enemy4Cards;

    public List<CardSO> PlayerCards;

    public void LoadNextScene()
    {
      
        //GameManager.Instance.UICanvas.SetActive(true);
        //GameManager.Instance.board.Clear();
        //GameManager.Instance.OccupiedEnemyPositions = 0;
        //GameManager.Instance.OccupiedPlayerPositions = 0;
        SceneManager.LoadScene(sceneCount);
        sceneCount++;
    }
}
