using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    private int transitionCount = 0;
    public static TransitionManager Instance;

    [SerializeField, Header("Transition Button")]
    private Button transitionButton;

    [SerializeField, Header("Animation Settings")]
    private Animator animator;
    [SerializeField, Tooltip("The name of the animations parameters in the animator controller")]
    private string[] transitionTriggers = { "Transition1", "Transition2", "Transition3" };

    [SerializeField, Header("Audio Settings")]
    private AudioSource audioSource;
    [SerializeField, Tooltip("Audio clips that match the transition animations (same order as triggers)")]
    private AudioClip[] transitionSounds;

    [SerializeField, Header("Loading Bar")]
    private GameObject loadingBarGO;
    [SerializeField]
    private Slider loadingSlider;

    [SerializeField, Header("Fake Loading Settings")]
    private bool enableFakeLoading = true;
    [SerializeField]
    private float fakeLoadingDuration = 3f;
    [SerializeField, Range(0f, 1f)]
    private float fakeLoadingChance = 0.33f;

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

            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Called when a scene transition is requested
    public void StartSceneTransition(string targetScene)
    {
        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogError("Target scene name is null or empty");
            return;
        }

        if (Application.CanStreamedLevelBeLoaded(targetScene))
        {
            nextSceneName = targetScene;
            hasStartedLoading = false;

            // Automatically find and disable the first active button under the EventSystem
            DisableActiveButton();

            int triggerIndex = Random.Range(0, transitionTriggers.Length);
            string trigger = transitionTriggers[triggerIndex];

            PlayTransitionSound(triggerIndex);
            animator.SetTrigger(trigger);
        }
        else
        {
            Debug.LogError($"Scene '{targetScene}' not found in build settings");
        }
    }



    // Plays the transition sound effect
    private void PlayTransitionSound(int triggerIndex)
    {
        if (audioSource != null && transitionSounds != null &&
            triggerIndex < transitionSounds.Length && transitionSounds[triggerIndex] != null)
        {
            audioSource.PlayOneShot(transitionSounds[triggerIndex]);
        }
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
        transitionCount++; // Increment transition counter

        // Every 3rd transition has a fake loading delay
        bool shouldDelay = enableFakeLoading && (transitionCount % 3 == 0);

        if (shouldDelay)
        {
            // Pause the transition animation
            if (animator)
                animator.speed = 0f;

            // Show loading bar and wait
            yield return StartCoroutine(ShowLoadingBarCoroutine());

            // Resume animation after delay
            if (animator)
                animator.speed = 1f;
        }

        SceneManager.LoadScene(nextSceneName);
        
        if (transitionButton)
            transitionButton.interactable = true;
    }


    // Handles displaying and updating the loading bar
    private IEnumerator ShowLoadingBarCoroutine()
    {
        if (loadingBarGO)
        {
            loadingBarGO.SetActive(true);
            loadingBarGO.transform.SetAsLastSibling(); 
        }

        float timer = 0f;
        while (timer < fakeLoadingDuration)
        {
            timer += Time.unscaledDeltaTime; 

            if (loadingSlider)
                loadingSlider.value = timer / fakeLoadingDuration;

            yield return null;
        }

        if (loadingBarGO)
            loadingBarGO.SetActive(false);
    }


    public void TriggerDelayCheck()
    {
        if (enableFakeLoading && Random.value < fakeLoadingChance)
        {
            StartCoroutine(LoadSceneCoroutine());
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void DisableActiveButton()
    {
        GameObject selectedObj = UnityEngine.EventSystems.EventSystem.current?.currentSelectedGameObject;

        if (selectedObj != null)
        {
            var button = selectedObj.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = false;
            }
        }
    }

}