using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;


public class EnemySpawn : MonoBehaviour
{
    public int EnemiesPerWave;
    public GameObject[] Enemies;
    
    public Transform[] SpawnPoints;
    public float TimeBetweenEnemies = 2f;

    private int _totalEnemiesInCurrentWave;
    private int _enemiesInWaveLeft;
    private int _spawnedEnemies;

    private int _currentWave;
    private int _totalWaves;

    void Start()
    {
        _currentWave = -1; // avoid off by 1
        _totalWaves = 999; // adjust, because we're using 0 index

        StartNextWave();
    }

    void StartNextWave()
    {
        _currentWave++;

        // win
        if (_currentWave > _totalWaves)
        {
            return;
        }

        _totalEnemiesInCurrentWave = EnemiesPerWave;
        _enemiesInWaveLeft = 0;
        _spawnedEnemies = 0;

        StartCoroutine(SpawnEnemies());
    }

    // Coroutine to spawn all of our enemies
    IEnumerator SpawnEnemies()
    {
        GameObject enemy = Enemies[Random.Range(0,Enemies.Length)];
        while (_spawnedEnemies < _totalEnemiesInCurrentWave)
        {
            _spawnedEnemies++;
            _enemiesInWaveLeft++;

            int spawnPointIndex = Random.Range(0, SpawnPoints.Length);

            // Create an instance of the enemy prefab at the randomly selected spawn point's 
            // position and rotation.
            //SpawnPoints[spawnPointIndex].rotation = UnityEngine.Quaternion.Euler(270, 180, 0);
            Instantiate(enemy, SpawnPoints[spawnPointIndex].position,
                             SpawnPoints[spawnPointIndex].rotation);
            yield return new WaitForSeconds(TimeBetweenEnemies);
        }
        StartNextWave();
        yield return null;
    }

    // called by an enemy when they're defeated
    public void EnemyDefeated()
    {
        _enemiesInWaveLeft--;

        // We start the next wave once we have spawned and defeated them all
        if (_enemiesInWaveLeft == 0 && _spawnedEnemies == _totalEnemiesInCurrentWave)
        {
            StartNextWave();
        }
    }
}
