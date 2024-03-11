using UnityEngine;

public class CreditsSceneLogic : MonoBehaviour
{

    private void Start()
    {
        GameManager.SetCreditsPlaying(true);
    }

    public void LoadMenu()
    {
        GameManager.SetCreditsPlaying(false);
        Time.timeScale = 1;
        GameManager.LoadMenu();
    }

    private void Update()
    {
        HandleEscape();
        if (Input.anyKey)
        {
            Time.timeScale += 3f * Time.unscaledDeltaTime;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    private void HandleEscape()
    {
#if (UNITY_EDITOR || UNITY_STANDALONE)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadMenu();
        }
#elif (UNITY_WEBGL)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            LoadMenu();
        }
#endif
    }
}
