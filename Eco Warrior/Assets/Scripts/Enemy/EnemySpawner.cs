using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    private Dictionary<int, int> enemySpawnerList = new Dictionary<int, int>
    {
        {1, 2},
        {2, 5},
        {3, 15},
    };
    private int level = 1;

    void Start()
    {
        StartCoroutine(Spawn());
    }

    void Update()
    {
    }

    IEnumerator Spawn()
    {
        int enemiesByLevel = enemySpawnerList.Where(a => a.Key == level).Select(b => b.Value).FirstOrDefault();
        var spawnPoint = GameObject.FindWithTag("EnemySpawnWaypoint");
        for (int i = 0; i < enemiesByLevel; i++)
        {
            Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(5f);
        }
    }
}
