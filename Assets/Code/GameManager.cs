using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private float DEFAULT_FIXED_TIMESTEP = 0.02f;

    private static string gameplayPersistentSceneName = "GAMEPLAY_PERSIST";

    private void Start()
    {
        Application.targetFrameRate = 60;
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

    private static int GetNonPersistentSceneBuildIndex()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name != gameplayPersistentSceneName)
            {
                return SceneManager.GetSceneAt(i).buildIndex;
            }
        }
        throw new System.Exception("Could not find non-persistent scene!");
    }

    public async static void ResetScene()
    {
        int index = GetNonPersistentSceneBuildIndex();
        Debug.Log(SceneManager.GetSceneByBuildIndex(index).name);
        if (!SceneManager.GetSceneByBuildIndex(index).isLoaded) return;
        await SceneManager.UnloadSceneAsync(index);
        SceneManager.LoadScene(index);
    }

    public async static void LoadNextScene()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Debug.Log(SceneManager.GetSceneAt(i).name);
        }

        int currentBuildIndex = GetNonPersistentSceneBuildIndex();

        if (SceneManager.GetSceneByBuildIndex(currentBuildIndex + 1).name == gameplayPersistentSceneName)
        {
            throw new System.Exception("Trying to load invalid scene! (Persistent one!)");
        }

        await SceneManager.UnloadSceneAsync(currentBuildIndex);
        SceneManager.LoadScene(currentBuildIndex + 1, LoadSceneMode.Additive);
    }

    private void Update()
    {
        Time.fixedDeltaTime = DEFAULT_FIXED_TIMESTEP * Time.timeScale;
    }
}
