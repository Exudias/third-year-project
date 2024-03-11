using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static string gameplayPersistentSceneName = "GAMEPLAY_PERSIST";

    private static bool loadingScene;
    private static bool playerDead;
    private static bool playingCutscene;
    private static bool playingCredits;

    public static GameManager Instance;

    private static Vector2 spawnPoint;
    private static bool setCustomSpawn;
    private static bool isFromPrevious;

    private InputManager input;

    private static float currentSceneTime;
    private static float currentSceneFrames;

    public delegate void GameEvent();
    public static event GameEvent OnGameStart;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            InitGameState();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    private const float PAUSE_LOCKOUT_TIME = 0.2f;

    private void Update()
    {
        KeyCode pauseKeyCode = KeyCode.Escape;
        KeyCode secondaryPauseKeyCode = KeyCode.Escape;
#if (UNITY_EDITOR || UNITY_STANDALONE)
        pauseKeyCode = KeyCode.Escape;
#elif (UNITY_WEBGL)
        pauseKeyCode = KeyCode.Return;
        secondaryPauseKeyCode = KeyCode.KeypadEnter;
#endif
        bool pauseLocked = currentSceneTime < PAUSE_LOCKOUT_TIME || playerDead;
        if (!pauseLocked && (input.IsDown(pauseKeyCode) || input.IsDown(secondaryPauseKeyCode)))
        {
            if (paused)
            {
                ResumeGame(true);
            }
            else
            {
                PauseGame();
            }
        }

        currentSceneTime += Time.unscaledDeltaTime;
        currentSceneFrames++;
    }

    private void InitGameState()
    {
        input = InputManager.instance;
        setCustomSpawn = false;
        isFromPrevious = false;
        Application.targetFrameRate = 60;
        Cursor.visible = false;
        loadingScene = false;
        playerDead = false;
        paused = false;
        timeScaleBeforePause = 1;
        currentSceneTime = 0;
        currentSceneFrames = 0;
    }

    private void Awake()
    {
        if (!HasPersistentScene())
        {
            LoadPersistentScene();
        }
    }

#region Scene Management

    private void LoadPersistentScene()
    {
        SceneManager.LoadScene(gameplayPersistentSceneName, LoadSceneMode.Additive);
    }

    private bool HasPersistentScene()
    {
        int scenes = SceneManager.sceneCount;
        for (int i = 0; i < scenes; i++)
        {
            if (SceneManager.GetSceneAt(i).name == gameplayPersistentSceneName)
            {
                return true;
            }
        }

        return false;
    }

    private static int GetCurrentLevelBuildIndex()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name.StartsWith("LVL_"))
            {
                return SceneManager.GetSceneAt(i).buildIndex;
            }
        }
        return -1;
    }

    public static Scene GetCurrentLevel()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name.StartsWith("LVL_"))
            {
                return SceneManager.GetSceneAt(i);
            }
        }
        throw new System.Exception("Could not find level scene!");
    }

    public static void MoveObjectToLevelScene(GameObject obj)
    {
        SceneManager.MoveGameObjectToScene(obj, GetCurrentLevel());
    }

    public void ResetScene()
    {
        LoadSceneByAdditiveID(0, false);
    }

    public async static void LoadSceneByID(int ID, bool resetSpawn = true)
    {
        if (loadingScene) return;

        loadingScene = true;

        timeScaleBeforePause = 1;
        ResumeGame();

        currentSceneTime = 0;
        currentSceneFrames = 0;

        float transitionTime = MenuManager.instance.PlayTransitionOut();

        await Awaitable.WaitForSecondsAsync(transitionTime);

        if (resetSpawn)
        {
            setCustomSpawn = false;
        }

        int currentLevelBuildIndex = GetCurrentLevelBuildIndex();

        // If there is no gameplay scene, simply load the scene without unloading anything
        if (currentLevelBuildIndex == -1)
        {
            SceneManager.LoadScene(ID);
            loadingScene = false;
            return;
        }

        if (SceneManager.GetSceneByBuildIndex(ID).name == gameplayPersistentSceneName)
        {
            throw new System.Exception("Trying to load invalid scene! (Persistent one!)");
        }

        currentSceneTime = 0;
        currentSceneFrames = 0;
        await SceneManager.UnloadSceneAsync(currentLevelBuildIndex);
        SceneManager.LoadScene(ID, LoadSceneMode.Additive);
        currentSceneTime = 0;
        currentSceneFrames = 0;

        loadingScene = false;
        playerDead = false;
    }

    public static void LoadSceneByAdditiveID(int addID, bool resetSpawn = true)
    {
        int currentLevelBuildIndex = GetCurrentLevelBuildIndex();
        if (currentLevelBuildIndex == -1)
        {
            currentLevelBuildIndex = SceneManager.GetActiveScene().buildIndex;
        }
        LoadSceneByID(currentLevelBuildIndex + addID, resetSpawn);
        // If starting from level 0, begin timer from start
        // TODO: Check if starting from save (not yet implemented)
        if (currentLevelBuildIndex == 0)
        {
            OnGameStart?.Invoke();
        }
    }

    public static void LoadPreviousScene()
    {
        isFromPrevious = true;
        LoadSceneByAdditiveID(-1);
    }

    public static void LoadNextScene()
    {
        isFromPrevious = false;
        LoadSceneByAdditiveID(1);
    }

    const string MAIN_MENU_SCENE_NAME = "MAIN_MENU";

    public async static void LoadMenu()
    {
        ResumeGame();

        float? timeMaybe = MenuManager.instance?.PlayTransitionOut();
        float transitionTime = timeMaybe != null ? (float)timeMaybe : 0;
        await Awaitable.WaitForSecondsAsync(transitionTime);

        currentSceneTime = 0;
        currentSceneFrames = 0;
        SceneManager.LoadScene(MAIN_MENU_SCENE_NAME);
    }

    const string CREDITS_SCENE_NAME = "CREDITS";

    public async static void LoadCredits()
    {
        ResumeGame();

        float? timeMaybe = MenuManager.instance?.PlayTransitionOut();
        float transitionTime = timeMaybe != null ? (float)timeMaybe : 0;
        await Awaitable.WaitForSecondsAsync(transitionTime);

        currentSceneTime = 0;
        currentSceneFrames = 0;
        SceneManager.LoadScene(CREDITS_SCENE_NAME);
    }

#endregion

#region Level Data
    public static void SetSpawn(Vector2 loc)
    {
        spawnPoint = loc;
        setCustomSpawn = true;
    }

    public static bool IsFromPrevious() => isFromPrevious;

    public static bool HasCustomSpawn() => setCustomSpawn;

    public static Vector2 GetCustomSpawnPoint() => spawnPoint;
#endregion

#region Game State
    private static bool paused = false;
    private static float timeScaleBeforePause = 1;

    // Credit to https://community.gamedev.tv/t/how-do-i-make-the-quit-button-work-for-webgl/40403/5
    public static void QuitGame()
    {
#if (UNITY_EDITOR)
            UnityEditor.EditorApplication.isPlaying = false;
#elif (UNITY_STANDALONE)
            Application.Quit();
#elif (UNITY_WEBGL)
            Application.OpenURL("https://exudias.itch.io/");
#endif
    }

    public void PauseGame()
    {
        if (MenuManager.instance == null) return;
        if (!MenuManager.instance.ShowPauseMenu()) return; // if can't pause, don't try
        if (playingCredits) return;
        if (Camera.main != null)
        {
            Camera.main.GetComponent<CameraManager>().SetIgnoreTimeScale(false);
        }
        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0;
        paused = true;
    }

    public static void ResumeGame(bool playSound = false)
    {
        if (Camera.main != null)
        {
            Camera.main.GetComponent<CameraManager>()?.SetIgnoreTimeScale(true);
        }
        Time.timeScale = timeScaleBeforePause;
        paused = false;
        MenuManager.instance?.HidePauseMenu(playSound);
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) return;

        bool pauseLocked = currentSceneTime < PAUSE_LOCKOUT_TIME || playerDead;
        if (pauseLocked) return;

        if (paused) return;

        PauseGame();
    }

    public static bool IsGamePaused() => paused;

    public static bool IsLoadingScene() => loadingScene;

    public static bool IsPlayerDead() => playerDead;

    public static void SetPlayerDead(bool newState)
    {
        playerDead = newState;
    }

    public static float GetSceneTime() => currentSceneTime;
    public static float GetSceneFrames() => currentSceneFrames;

    public static void SetCutscenePlaying(bool newState)
    {
        playingCutscene = newState;
    }
    public static bool GetPlayingCutscene() => playingCutscene;

    public static void SetCreditsPlaying(bool newState)
    {
        playingCredits = newState;
    }

    public static bool GetCreditsPlaying() => playingCredits;
#endregion
}
