using UnityEngine;

public class SpiritDeathLogic : MonoBehaviour
{
    [SerializeField] private GameObject deathParticles;

    public void SpawnDeathParticles()
    {
        GameObject deathParticlesObj = Instantiate(deathParticles, transform.position, Quaternion.identity);
        GameManager.MoveObjectToLevelScene(deathParticlesObj);
    }

    private void Start()
    {
        SpawnDeathParticles();
    }
}
