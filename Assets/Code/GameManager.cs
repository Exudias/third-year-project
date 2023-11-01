using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private float DEFAULT_FIXED_TIMESTEP = 0.02f;

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    public static void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void Update()
    {
        Time.fixedDeltaTime = DEFAULT_FIXED_TIMESTEP * Time.timeScale;
    }
}
