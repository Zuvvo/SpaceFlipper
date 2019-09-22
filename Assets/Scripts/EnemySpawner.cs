using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private EnemyBase enemy;
    public bool IsOccupied { get; private set; }

    private void Start()
    {
        EnemyController.Instance.RegisterSpawner(this);
    }

    public EnemyBase TrySpawn(EnemyBase prefab)
    {
        if (!IsOccupied)
        {
            enemy = Instantiate(prefab, transform.position, Quaternion.identity);
            enemy.Init(this);
            IsOccupied = true;
            return enemy;
        }
        return null;
    }

    public void TryToDestroy(EnemyBase enemyBase)
    {
        Destroy(enemyBase.gameObject);
        IsOccupied = false;
    }
}
