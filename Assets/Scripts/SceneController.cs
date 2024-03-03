using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    #region Singleton

    private static SceneController instance;

    public static SceneController Instance { 
        get {
            return instance;
        }
    }

    #endregion


    [Header("Panels")]
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject aboutGamePanel;
    [SerializeField] GameObject characterSelectionPanel;
    [SerializeField] GameObject loadingPanel;
    [SerializeField] GameObject fadeImage;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject backButton;
    [SerializeField] TMP_Text titleText;
    [Space(10)]

    [Header("Mask Settings")]
    [SerializeField] GameObject maskButton;
    [SerializeField] GameObject maskGameObject;
    [SerializeField] GameObject LeftMaskGameObject;
    [SerializeField] GameObject RightMaskGameObject;
    [SerializeField] Animator maskAnimator;
    [Space(10)]

    [SerializeField] Slider loadingBar;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float loadingDelta = 0.1f;

    private Animator animator;
    private int levelToLoad = 0;
    private float target = 0;

    private bool isGamePaused = false;
    private bool isMainMenuActive = true;
    private bool isPlaySceneActive = false;
    private bool isCutscenePlaying = false;

    private bool isCharacterSelectionShouldBeActive = false;
    private bool isAboutGameShouldBeActive = false;
    private bool isLoadGameShouldBeActive = false;

    public bool IsCharacterSelectionShouldBeActive
    {
        get { return isCharacterSelectionShouldBeActive; }
    }

    public bool IsAboutGameShouldBeActive
    {
        get { return isAboutGameShouldBeActive; }
    }

    public bool IsLoadGameShouldBeActive
    {
        get { return isLoadGameShouldBeActive; }
    }


    void Awake()
    {
        if(instance == null) 
        {
            instance = this as SceneController;
            DontDestroyOnLoad(gameObject);
        } 
        else 
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (loadingPanel.activeSelf)
        {
            loadingBar.value = Mathf.MoveTowards(loadingBar.value, target, loadingDelta * Time.deltaTime);
        }

        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            if(!pausePanel.activeSelf && !isMainMenuActive && !loadingPanel.activeSelf 
                && !fadeImage.activeSelf && isPlaySceneActive)
            {
                SetPauseScreen(true);
            }
            else if(pausePanel.activeSelf)
            {
                SetPauseScreen(false);
            }

            if(isCutscenePlaying)
            {
                /*foreach (var audio in FindObjectsOfType<AudioSource>())
                {
                    StartCoroutine(StartFadeAudio(1.0f, 0, audio));
                }*/
            }
        }
    }


    #region Public Methods

    public void OnMaskClicked()
    {
        maskButton.SetActive(false);
        maskGameObject.SetActive(true);
    }

    public void OnLoadMainMenu()
    {
        isMainMenuActive = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Time.timeScale = 1f;
        FadeAndLoadScene(0);
    }

    public void OnLoadGameAnimCliced()
    {
        isAboutGameShouldBeActive = false;
        isCharacterSelectionShouldBeActive = false;
        isLoadGameShouldBeActive = true;
        OnAnimButtonClicked();
    }

    public void LoadGame()
    {
        FadeAndLoadScene(1);
    }

    public void ContinueGame()
    {
        SetPauseScreen(false);
    }

    public void OnAboutAnimButtonClicked()
    {
        isAboutGameShouldBeActive = true;
        isCharacterSelectionShouldBeActive = false;
        isLoadGameShouldBeActive = false;
        OnAnimButtonClicked();
    }

    public void OnCharacterSelectionAnimButtonClicked()
    {
        isAboutGameShouldBeActive = false;
        isCharacterSelectionShouldBeActive = true;
        isLoadGameShouldBeActive = false;
        OnAnimButtonClicked();
    }

    public void OnAnimButtonClicked()
    {
        LeftMaskGameObject.GetComponent<Animator>().SetTrigger("IsCombining");
        RightMaskGameObject.GetComponent<Animator>().SetTrigger("IsCombining");
        mainMenuPanel.GetComponent<Animator>().SetTrigger("IsCombining");
    }

    public void AboutGame()
    {
        mainMenuPanel.SetActive(false);
        LeftMaskGameObject.SetActive(false);
        RightMaskGameObject.SetActive(false);
        aboutGamePanel.SetActive(true);

        titleText.text = "About";
        backButton.SetActive(true);
    }

    public void CharacterSelection()
    {
        mainMenuPanel.SetActive(false);
        LeftMaskGameObject.SetActive(false);
        RightMaskGameObject.SetActive(false);
        characterSelectionPanel.SetActive(true);

        titleText.text = "Character Selection";
        backButton.SetActive(true);
    }

    public void OnBackGame()
    {
        aboutGamePanel.SetActive(false);
        characterSelectionPanel.SetActive(false);
        LeftMaskGameObject.SetActive(true);
        RightMaskGameObject.SetActive(true);
        mainMenuPanel.SetActive(true);
        
        titleText.text = "The Clowning Of Eternity";
        backButton.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    public void OnFadeOutComplete()
    {
        LoadSceneAsync();
    }

    #endregion


    #region Private Methods

    private void SetPauseScreen(bool setActive)
    {
        pausePanel.SetActive(setActive);
        foreach (var obj in FindObjectsOfType<MouseRotator>())
        {
            if(setActive)
            {
                obj.enabled = false;
                //obj.gameObject.SetActive(false);
            }
            else
            {
                obj.enabled = true;
                //obj.gameObject.SetActive(true);
            }
        }
        isGamePaused = setActive;

        if(setActive)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
        }
        Cursor.visible = setActive;
    }

    private async void LoadSceneAsync()
    {
        loadingBar.value = 0;
        target = 0;

        AsyncOperation operation = SceneManager.LoadSceneAsync(levelToLoad);
        operation.allowSceneActivation = false;
        loadingPanel.SetActive(true);

        do
        {
            await Task.Delay(1000);
            target = operation.progress;
        }
        while (loadingBar.value < 0.9f);

        target = 1;
        await Task.Delay(1000);

        operation.allowSceneActivation = true;
        loadingPanel.SetActive(false);
        animator.SetBool("IsFadingOut", false);

        isMainMenuActive = levelToLoad == 0;
        isPlaySceneActive = levelToLoad != 0;
    }

    private void FadeAndLoadScene(int sceneBuildIndex)
    {
        levelToLoad = sceneBuildIndex;
        animator.SetBool("IsFadingOut", true);
        //StartCoroutine(StartFadeAudio(1.0f, 0));
    }

    private IEnumerator StartFadeAudio(float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    #endregion
}
