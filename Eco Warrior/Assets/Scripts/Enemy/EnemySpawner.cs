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
    private int level;
    private Dictionary<int, int> _enemySpawnerList = new Dictionary<int, int>
    {
        {1, 3},
        {2, 5},
        {3, 7},
    };

    private void Awake()
    {
        _destroyedObjects = FindObjectOfType<DestroyedObjects>();
        if (_destroyedObjects == null)
        {
            Debug.LogError("DestroyedObjects not found in scene!");
            return;
        }

        level = _destroyedObjects.level;
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
