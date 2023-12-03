using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    private static string gameplayPersistentSceneName = "GAMEPLAY_PERSIST";

    private static bool loadingScene;

    private static GameManager Instance;

    private static Vector2 spawnPoint;
    private static bool setCustomSpawn;

    private InputManager input;

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

    private void Update()
    {
        if (input.IsDown(KeyCode.Escape))
        {
            if (paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void InitGameState()
    {
        input = InputManager.instance;
        setCustomSpawn = false;
        Application.targetFrameRate = 60;
        loadingScene = false;
        paused = false;
        timeScaleBeforePause = 1;
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
        throw new System.Exception("Could not find level scene!");
    }

    private static Scene GetCurrentLevel()
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

    public async static void ResetScene()
    {
        ResumeGame();
        int currentLevel = GetCurrentLevelBuildIndex();
        if (!SceneManager.GetSceneByBuildIndex(currentLevel).isLoaded) return;
        await SceneManager.UnloadSceneAsync(currentLevel);
        SceneManager.LoadScene(currentLevel);
    }

    public async static void LoadNextScene()
    {
        setCustomSpawn = false;

        if (loadingScene) return;

        loadingScene = true;

        int currentLevelBuildIndex = GetCurrentLevelBuildIndex();

        if (SceneManager.GetSceneByBuildIndex(currentLevelBuildIndex + 1).name == gameplayPersistentSceneName)
        {
            throw new System.Exception("Trying to load invalid scene! (Persistent one!)");
        }

        await SceneManager.UnloadSceneAsync(currentLevelBuildIndex);
        SceneManager.LoadScene(currentLevelBuildIndex + 1, LoadSceneMode.Additive);

        loadingScene = false;
    }

    #endregion

    #region Level Data
    public static void SetSpawn(Vector2 loc)
    {
        spawnPoint = loc;
        setCustomSpawn = true;
    }

    public static bool HasCustomSpawn() => setCustomSpawn;

    public static Vector2 GetCustomSpawnPoint() => spawnPoint;
    #endregion

    #region Game State
    private static bool paused = false;
    private static float timeScaleBeforePause = 1;

    public static void PauseGame()
    {
        if (Camera.main != null)
        {
            Camera.main.GetComponent<CameraManager>().SetIgnoreTimeScale(false);
        }
        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0;
        paused = true;
    }

    public static void ResumeGame()
    {
        if (Camera.main != null)
        {
            Camera.main.GetComponent<CameraManager>().SetIgnoreTimeScale(true);
        }
        Time.timeScale = timeScaleBeforePause;
        paused = false;
    }

    public static bool IsGamePaused() => paused;
    #endregion
}
