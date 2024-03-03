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
    [SerializeField] GameObject pauseButtonPanel;
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
    [SerializeField] GameObject LeftPauseMaskGameObject;
    [SerializeField] GameObject RightPauseMaskGameObject;
    [SerializeField] Animator maskAnimator;
    [Space(10)]

    [SerializeField] Slider loadingBar;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float loadingDelta = 0.1f;

    private Animator animator;
    private WeaponSystem weaponSystems = null;
    private int levelToLoad = 0;
    private float target = 0;

    private bool isGamePaused = false;
    private bool isMainMenuActive = true;
    private bool isPlaySceneActive = false;
    private bool isCutscenePlaying = false;

    private bool isCharacterSelectionShouldBeActive = false;
    private bool isAboutGameShouldBeActive = false;
    private bool isLoadGameShouldBeActive = false;
    private bool contuineShouldHappen = false;
    private bool loadMainMenuShouldHappen = false;
    private bool quitGameShouldHappen = false;

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

    public bool ContuineShouldHappen
    {
        get { return contuineShouldHappen; }
    }

    public bool LoadMainMenuShouldHappen
    {
        get { return loadMainMenuShouldHappen; }
    }

    public bool QuitGameShouldHappen
    {
        get { return quitGameShouldHappen; }
    }


    void Awake()
    {
        if(instance == null) 
        {
            instance = this as SceneController;
        } 
        else 
        {
            SceneController old = instance;
            instance = this as SceneController;
            old.gameObject.SetActive(false);
            Destroy(old.gameObject);
        }
        DontDestroyOnLoad(gameObject);
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
            PauseGame();
        }
    }
    public void PauseGame()
    {
        if (!pausePanel.activeSelf && !isMainMenuActive && !loadingPanel.activeSelf
                && !fadeImage.activeSelf && isPlaySceneActive)
        {
            SetPauseScreen(true);
        }
        else if (pausePanel.activeSelf)
        {
            AnimContinueGame();
        }

        if (isCutscenePlaying)
        {
            /*foreach (var audio in FindObjectsOfType<AudioSource>())
            {
                StartCoroutine(StartFadeAudio(1.0f, 0, audio));
            }*/
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
        pausePanel.SetActive(false);
        isMainMenuActive = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Time.timeScale = 0f;
        FadeAndLoadScene(0);
    }

    public void AnimOnLoadMainMenu()
    {
        isAboutGameShouldBeActive = false;
        isCharacterSelectionShouldBeActive = false;
        isLoadGameShouldBeActive = false;
        contuineShouldHappen = false;
        loadMainMenuShouldHappen = true;
        quitGameShouldHappen = false;
        OnPauseAnimButtonClicked();
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

    public void AnimContinueGame()
    {
        isAboutGameShouldBeActive = false;
        isCharacterSelectionShouldBeActive = false;
        isLoadGameShouldBeActive = false;
        contuineShouldHappen = true;
        loadMainMenuShouldHappen = false;
        quitGameShouldHappen = false;
        OnPauseAnimButtonClicked();
    }

    public void OnAboutAnimButtonClicked()
    {
        isAboutGameShouldBeActive = true;
        isCharacterSelectionShouldBeActive = false;
        isLoadGameShouldBeActive = false;
        contuineShouldHappen = false;
        loadMainMenuShouldHappen = false;
        quitGameShouldHappen = false;
        OnAnimButtonClicked();
    }

    public void OnCharacterSelectionAnimButtonClicked()
    {
        isAboutGameShouldBeActive = false;
        isCharacterSelectionShouldBeActive = true;
        isLoadGameShouldBeActive = false;
        contuineShouldHappen = false;
        loadMainMenuShouldHappen = false;
        quitGameShouldHappen = false;
        OnAnimButtonClicked();
    }

    public void OnAnimButtonClicked()
    {
        LeftMaskGameObject.GetComponent<Animator>().SetTrigger("IsCombining");
        RightMaskGameObject.GetComponent<Animator>().SetTrigger("IsCombining");
        mainMenuPanel.GetComponent<Animator>().SetTrigger("IsCombining");
    }

    public void OnPauseAnimButtonClicked()
    {
        LeftPauseMaskGameObject.GetComponent<Animator>().SetTrigger("IsCombining");
        RightPauseMaskGameObject.GetComponent<Animator>().SetTrigger("IsCombining");
        pauseButtonPanel.GetComponent<Animator>().SetTrigger("IsCombining");
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

    public void AnimQuitGame()
    {
        isAboutGameShouldBeActive = false;
        isCharacterSelectionShouldBeActive = false;
        isLoadGameShouldBeActive = false;
        contuineShouldHappen = false;
        loadMainMenuShouldHappen = false;
        quitGameShouldHappen = true;

        OnPauseAnimButtonClicked();
    }

    public void OnFadeOutComplete()
    {
        LoadSceneAsync();
    }

    #endregion


    #region Private Methods

    public void SetPauseScreen(bool setActive)
    {
        pausePanel.SetActive(setActive);
        
        
        if(setActive)
        {
            weaponSystems = FindObjectOfType<WeaponSystem>();
            weaponSystems.gameObject.SetActive(false);
        }
        else
        {
            weaponSystems.gameObject.SetActive(true);
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
        Time.timeScale = 1f;

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
        StartCoroutine(StartFadeAudio(1.0f, 0));
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
