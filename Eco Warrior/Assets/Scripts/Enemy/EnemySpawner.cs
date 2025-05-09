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
        {2, 2},
        {3, 4},
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

        int pistolCount = Mathf.CeilToInt(enemiesToSpawn / 2f);
        int smgCount = enemiesToSpawn - pistolCount;

        Debug.Log($"Spawning {pistolCount} pistols and {smgCount} SMGs");

        for (int i = 0; i < pistolCount; i++)
        {
            Instantiate(_enemyPrefabPistol, spawnPoint.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
        }

        for (int i = 0; i < smgCount; i++)
        {
            Instantiate(_enemyPrefabSMG, spawnPoint.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
