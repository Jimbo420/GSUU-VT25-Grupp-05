using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject prefab; // The prefab to pool
    [SerializeField] private int initialSize = 10; // Initial number of objects in the pool

    private readonly Queue<GameObject> pool = new();

    private void Awake()
    {
        // Pre-instantiate objects and add them to the pool
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    // Public method to set the prefab
    public void SetPrefab(GameObject prefab)
    {
        this.prefab = prefab;
    }

    // Public method to set the initial size
    public void SetInitialSize(int size)
    {
        initialSize = size;
    }

    public GameObject GetFromPool(Vector3 position, Quaternion rotation)
    {
        GameObject obj;

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            // If the pool is empty, instantiate a new object
            obj = Instantiate(prefab);
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);

        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}

