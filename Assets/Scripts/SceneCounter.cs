using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCounter : Singleton<SceneCounter>
{
   public int sceneCount = 1;

   public void LoadNextScene()
   {
        SceneManager.LoadScene(sceneCount);
        sceneCount++;
   }
}
