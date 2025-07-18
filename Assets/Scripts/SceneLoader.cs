using UnityEngine;

public class SceneLoader : MonoBehaviour
{


    public void SwitchScene()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string targetScene = currentScene == "SceneA" ? "SceneB" : "SceneA";

        TransitionManager.Instance.StartSceneTransition(targetScene);
    }
}
