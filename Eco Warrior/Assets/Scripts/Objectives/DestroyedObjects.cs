using UnityEngine;

public class DestroyedObjects : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int _gameObjectsDestroyed;
    public int level = 1;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameObjectsDestroyed > 2)
            level += 1;
        Debug.Log(level);

    }
}
