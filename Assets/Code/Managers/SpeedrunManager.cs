using UnityEngine;

public class SpeedrunManager : MonoBehaviour
{
    public static float time = 0;

    private void OnEnable()
    {
        GameManager.OnGameStart += OnBeginGame;
    }

    private void OnDisable()
    {
        GameManager.OnGameStart -= OnBeginGame;
    }

    private void OnBeginGame()
    {
        time = 0;
    }

    private void Update()
    {
        bool paused = GameManager.IsGamePaused();
        bool transitioning = GameManager.IsLoadingScene();
        if (!paused && !transitioning)
        {
            time += Time.unscaledDeltaTime;
        }
    }
}
