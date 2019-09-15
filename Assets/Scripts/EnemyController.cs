using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyBase EnemyPrefab;
    public float SpawnDelay;

    private List<EnemyBase> spawnedEnemies = new List<EnemyBase>();
    private List<EnemySpawner> enemySpawners = new List<EnemySpawner>();

    private static EnemyController _instance;
    public static EnemyController Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<EnemyController>();
            }
            return _instance;
        }
    }

    private void Start()
    {
        StartCoroutine(EnemySpawnRoutine());
    }

    private IEnumerator EnemySpawnRoutine()
    {
        while (true)
        {
            List<EnemySpawner> notOccupiedSpawners = new List<EnemySpawner>(enemySpawners.FindAll(x => !x.IsOccupied));
            if (notOccupiedSpawners.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, notOccupiedSpawners.Count - 1);
                notOccupiedSpawners[randomIndex].TrySpawn(EnemyPrefab);
            }
            yield return new WaitForSeconds(SpawnDelay);
        }
    }

    public void RegisterSpawner(EnemySpawner spawner)
    {
        enemySpawners.Add(spawner);
    }
}