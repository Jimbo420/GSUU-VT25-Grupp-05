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
    private DestroyedObjects _destroyedObjects;
    private int _lastLevel;
    private Dictionary<int, int> _enemySpawnerList = new Dictionary<int, int>
    {
        {1, 1},
        {2, 4},
        {3, 6},
    };

    private void Awake()
    {
        _destroyedObjects = GetComponent<DestroyedObjects>();
        _lastLevel = _destroyedObjects.level;
        StartCoroutine(Spawn());
    }

    void Update()
    {
        if (_lastLevel != _destroyedObjects.level)
        {
            _lastLevel = _destroyedObjects.level;
            StartCoroutine(Spawn());
        }
    }
    IEnumerator Spawn()
    {
        int enemiesToSpawn = 0;

        if (_enemySpawnerList.ContainsKey(_destroyedObjects.level))
            enemiesToSpawn = _enemySpawnerList[_destroyedObjects.level];

        var spawnPoint = GameObject.FindWithTag("EnemySpawnWaypoint");

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            GameObject enemyToSpawn = _enemyPrefabPistol;

            if (i < _destroyedObjects.level)
                enemyToSpawn = _enemyPrefabSMG;

            Instantiate(enemyToSpawn, spawnPoint.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(5f);
        }
    }
}
