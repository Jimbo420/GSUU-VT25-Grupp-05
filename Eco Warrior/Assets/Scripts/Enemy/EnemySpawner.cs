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
    public int enemyLevel = 2;
    private Dictionary<int, int> enemySpawnerList = new Dictionary<int, int>
    {
        {1, 2},
        {2, 5},
        {3, 15},
    };

    void Start()
    {
        StartCoroutine(Spawn());
    }

    void Update()
    {
    }

    IEnumerator Spawn()
    {
        int enemiesByLevel = enemySpawnerList.Where(a => a.Key == enemyLevel).Select(b => b.Value).FirstOrDefault();
        var spawnPoint = GameObject.FindWithTag("EnemySpawnWaypoint");
        for (int i = 0; i < enemiesByLevel; i++)
        {
            Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
            HealthbarBehavior healthbarBehavior = new HealthbarBehavior();
            //healthbarBehavior.SetHealth(enemyLevel);
            yield return new WaitForSeconds(5f);
        }
    }
}
