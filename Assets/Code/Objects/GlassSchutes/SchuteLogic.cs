using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchuteLogic : MonoBehaviour
{
    [SerializeField] private GameObject glassShardPrefab;
    [SerializeField] private Transform topSchute;
    [SerializeField] private Transform botSchute;

    [SerializeField] private int shardCount = 8;

    [SerializeField] private float shardTimeThrough = 0.2f;

    [SerializeField] private Vector2 maxOffset = new Vector2(0.2f, 0.2f);

    private float timeBetweenShardSpawn;
    private int shards;
    private float timeSinceShardSpawn;

    private void Start()
    {
        timeBetweenShardSpawn = shardTimeThrough / shardCount;
        shards = 0;
        timeSinceShardSpawn = 0;
    }

    private void Update()
    {
        if (shards < shardCount)
        {
            timeSinceShardSpawn += Time.deltaTime;
            if (timeSinceShardSpawn > timeBetweenShardSpawn)
            {
                SpawnShard();
            }
        }
    }

    private void SpawnShard()
    {
        timeSinceShardSpawn = 0;
        shards++;
        Vector3 offset = new Vector2((Random.value - 0.5f) * 2 * maxOffset.x, (Random.value - 0.5f) * 2 * maxOffset.y);
        GlassShardLogic shard = Instantiate(glassShardPrefab, topSchute.position + offset, Quaternion.identity).GetComponent<GlassShardLogic>();
        shard.Initialize(topSchute.position + offset, botSchute.position + offset, shardTimeThrough);
        GameManager.MoveObjectToLevelScene(shard.gameObject);
    }
}
