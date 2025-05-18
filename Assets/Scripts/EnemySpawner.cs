using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 0.5f;

    private float nextSpawnTime;

    void Start()
    {
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime += spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * 12f;
        Instantiate(enemyPrefab, randomPos, Quaternion.identity);
    }
}
