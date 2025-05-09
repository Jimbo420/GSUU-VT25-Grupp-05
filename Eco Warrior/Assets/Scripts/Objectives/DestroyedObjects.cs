using UnityEngine;

public class DestroyedObjects : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int _gameObjectsDestroyed;
    public int level = 1;
    void Start()
    {
    }

    private void Update()
    {
        if (_gameObjectsDestroyed == level && level < 3)
            level++;
    }

    public int TotalDestoyedObjects()
    {
        return _gameObjectsDestroyed;
    }
}
