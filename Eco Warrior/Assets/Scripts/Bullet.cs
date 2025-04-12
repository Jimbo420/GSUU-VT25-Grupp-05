using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float maxDistance = 10f;
    private Vector3 _startPosition;

    void Start()
    {
        _startPosition = transform.position;
    }

    void Update()
    {
        float distanceTravelled = Vector3.Distance(_startPosition, transform.position);
        if (distanceTravelled >= maxDistance)
        {
            Destroy(gameObject);
        }
    }
}
