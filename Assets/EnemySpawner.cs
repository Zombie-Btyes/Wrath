using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject ZombiePrefab;

    public float _miniSpawnTime = 2f;
    public float _maxSpawnTime = 5f;
    public int maxSpawnCount = 50;
    public int _spawnCount = 0;

    private float _timeUntilSpawn;

    void Awake()
    {
        SetTimeUntilSpawn();
    }

    void Update()
    {
        _timeUntilSpawn -= Time.deltaTime;

        if (_timeUntilSpawn <= 0 && _spawnCount < maxSpawnCount)
        {
            SpawnEnemy();
            SetTimeUntilSpawn();
        }
    }

    private void SpawnEnemy()
    {
        // Instantiate the zombie
        GameObject zombie = Instantiate(ZombiePrefab, transform.position, Quaternion.identity);

        // Set a random rotation for the zombie
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        zombie.transform.rotation = Quaternion.LookRotation(randomDirection);

        _spawnCount++;
    }

    private void SetTimeUntilSpawn()
    {
        _timeUntilSpawn = Random.Range(_miniSpawnTime, _maxSpawnTime);
    }
}