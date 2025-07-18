using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{

    public GameObject loadingBarGO;   
    public Slider loadingSlider;
    public static TransitionManager Instance;

    public Animator animator;
    public string[] transitionTriggers = { "Transition1", "Transition2", "Transition3" };

    public bool enableFakeLoading = true;
    public float fakeLoadingDuration = 3f;
    [Range(0f, 1f)]
    public float fakeLoadingChance = 0.33f;

    private string nextSceneName;
    private bool hasStartedLoading = false;

    // Initializes the singleton instance and references the Animator
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (animator == null)
                animator = GetComponent<Animator>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Called when a scene transition is requested
    public void StartSceneTransition(string targetScene)
    {
        nextSceneName = targetScene;
        hasStartedLoading = false;

        string trigger = transitionTriggers[Random.Range(0, transitionTriggers.Length)];
        animator.SetTrigger(trigger);
    }

    // Called from an animation event to begin loading the new scene
    public void LoadSceneNow()
    {
        if (hasStartedLoading) return;

        hasStartedLoading = true;
        StartCoroutine(LoadSceneCoroutine());
    }

    // Coroutine that handles optional loading delay before switching scenes
    private IEnumerator LoadSceneCoroutine()
    {
        if (enableFakeLoading && Random.value < fakeLoadingChance)
        {
            // Pause the transition animation
            if (animator != null)
                animator.speed = 0f;

            if (loadingBarGO != null)
                loadingBarGO.SetActive(true);

                 loadingBarGO.SetActive(true);
                 loadingBarGO.transform.SetAsLastSibling();


            float timer = 0f;
            while (timer < fakeLoadingDuration)
            {
                timer += Time.deltaTime;

                // Fill loading bar in real time
                if (loadingSlider != null)
                    loadingSlider.value = timer / fakeLoadingDuration;

                yield return null;
            }

            if (loadingBarGO != null)
                loadingBarGO.SetActive(false);

            // Resume the transition animation
            if (animator != null)
                animator.speed = 1f;
        }

        SceneManager.LoadScene(nextSceneName);
    }


    // Pause the transition on fake delay
    public void TriggerDelayCheck()
    {
        Debug.Log("TriggerDelayCheck called from animation");

    }

}
