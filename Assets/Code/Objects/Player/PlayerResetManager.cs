using UnityEngine;

public class PlayerResetManager : MonoBehaviour
{
    private PlayerLogic logic;

    private void Awake()
    {
        logic = GetComponent<PlayerLogic>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TryResetLevel();
        }
    }

    private void TryResetLevel()
    {
        if (GameManager.IsLoadingScene()) return;
        if (GameManager.IsPlayerDead()) return;
        if (GameManager.IsGamePaused()) return;

        logic.Die();
    }
}
