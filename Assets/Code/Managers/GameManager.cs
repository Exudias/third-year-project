using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static string gameplayPersistentSceneName = "GAMEPLAY_PERSIST";

    private static bool loadingScene;

    private void Start()
    {
        Application.targetFrameRate = 60;
        loadingScene = false;
    }

    private void Awake()
    {
        if (!HasPersistentScene())
        {
            LoadPersistentScene();
        }
    }

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
        int currentLevel = GetCurrentLevelBuildIndex();
        if (!SceneManager.GetSceneByBuildIndex(currentLevel).isLoaded) return;
        await SceneManager.UnloadSceneAsync(currentLevel);
        SceneManager.LoadScene(currentLevel);
    }

    public async static void LoadNextScene()
    {
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
}
