using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public int _enemyLevel = 2;
    private Dictionary<int, int> EnemySpawnerList = new Dictionary<int, int>
    {
        {1, 2},
        {2, 3},
        {3, 5},
    };

    [SerializeField] GameObject _enemyPrefab;

    private WeaponManager _weaponManager;
    private WeaponUI _weaponUI;
    //private WeaponVisuals _weaponVisuals;

    void Start()
    {
        StartCoroutine(Spawn());
    }

    void Update()
    {
    }

    IEnumerator Spawn()
    {
        int enemiesByLevel = EnemySpawnerList.Where(a => a.Key == _enemyLevel).Select(b => b.Value).FirstOrDefault();
        var spawnPoint = GameObject.FindWithTag("EnemySpawnWaypoint");
        if ( spawnPoint == null)
        {
            Debug.Log("NUll spawnpoint");
        }
        for (int i = 0; i < enemiesByLevel; i++)
        {
            if (_enemyLevel >= i)
            {
                WeaponManager _weaponManager = _enemyPrefab.GetComponentInChildren<WeaponManager>();
                var test = _weaponManager.GetWeapon(0);
                //Debug.Log("Vapen: " + test);
                _weaponManager.SwitchWeapon(test);
                Instantiate(_enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
                

                //WeaponVisuals weaponVisuals = _enemyPrefab.GetComponentInChildren<WeaponVisuals>();
                //weaponVisuals.UpdateWeaponSprite();
            }
            yield return new WaitForSeconds(5f);
        }
    }
}
