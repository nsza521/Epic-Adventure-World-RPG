using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public GameObject[] enemies;
        public int enemyCount;
        public float spawnRate;
    }

    public Wave[] waves;
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 5f; 

    private int currentWaveIndex = 0;
    private bool spawningWave = false;

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (true)
        {
            if (!spawningWave)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
                spawningWave = true;

                Wave waveToSpawn = (currentWaveIndex >= waves.Length - 1)
                    ? waves[waves.Length - 1]
                    : waves[currentWaveIndex];

                StartCoroutine(SpawnWave(waveToSpawn));
            }
            yield return null;
        }
    }

    IEnumerator SpawnWave(Wave wave)
    {
        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy(wave);
            yield return new WaitForSeconds(1f / wave.spawnRate);
        }

        yield return new WaitForSeconds(timeBetweenWaves);

        spawningWave = false;

        if (currentWaveIndex < waves.Length - 1)
        {
            currentWaveIndex++;
        }
    }

    void SpawnEnemy(Wave wave)
    {
        int randomEnemyIndex = Random.Range(0, wave.enemies.Length);
        int randomSpawnPointIndex = Random.Range(0, spawnPoints.Length);

        GameObject enemyPrefab = wave.enemies[randomEnemyIndex];
        Transform spawnPoint = spawnPoints[randomSpawnPointIndex];

        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
