using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] public Transform[] WayPoints;
    private int amountOfEnemies;
    private int level = 1;
    private Dictionary<int, int> enemySpawnerList = new Dictionary<int, int>
    {
        {1, 2},
        {2, 10},
        {3, 15},
    };
    void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        int enemiesByLevel = enemySpawnerList.Where(a => a.Key.Equals(level)).Select(b => b.Value).FirstOrDefault();
        for (int i = 0; i >= enemiesByLevel; i++)
        {
            Instantiate(enemyPrefab, WayPoints[i].position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
