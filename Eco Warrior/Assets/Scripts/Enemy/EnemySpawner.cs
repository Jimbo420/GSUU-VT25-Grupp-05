using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject _enemyPrefabPistol;
    [SerializeField] GameObject _enemyPrefabSMG;
    private Dictionary<int, int> _enemySpawnerList = new Dictionary<int, int>
    {
        {1, 3},
        {2, 5},
        {3, 7},
    };
    private int level;

    private void Awake()
    {
        level = 2;
        StartCoroutine(Spawn());

    }

    void Start()
    {
    }

    void Update()
    {
    }

    IEnumerator Spawn()
    {
        int enemiesToSpawn = 0;

        if (_enemySpawnerList.ContainsKey(level))
            enemiesToSpawn = _enemySpawnerList[level];

        var spawnPoint = GameObject.FindWithTag("EnemySpawnWaypoint");

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            GameObject enemyToSpawn = _enemyPrefabPistol;

            if (i < level)
                enemyToSpawn = _enemyPrefabSMG;

            Instantiate(enemyToSpawn, spawnPoint.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(5f);
        }
    }
}
