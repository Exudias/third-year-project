using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private float DEFAULT_FIXED_TIMESTEP = 0.02f;   

    public static void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        Time.fixedDeltaTime = DEFAULT_FIXED_TIMESTEP * Time.timeScale;
    }
}
