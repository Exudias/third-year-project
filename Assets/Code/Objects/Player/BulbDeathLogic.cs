using UnityEngine;

public class BulbDeathLogic : MonoBehaviour
{
    [SerializeField] private GameObject deathParticles;

    public void SpawnDeathParticles()
    {
        GameObject deathParticlesObj = Instantiate(deathParticles, transform.position, Quaternion.identity);
        GameManager.MoveObjectToLevelScene(deathParticlesObj);
    }
}
