using Interfaces;
using UnityEngine;
public class Bullet : MonoBehaviour
{
    public float maxDistance = 10f; //Maximum of distance that the bullet can travel
    private Vector3 _startPosition;
    private float _damage;
    private GameObject _shooter;

    public void SetDamage(float damage)
    {
        _damage = damage;
    }

    public void SetShooter(GameObject shooter)
    {
        _shooter = shooter;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == _shooter) return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable == null) return;

        damageable.HitDamage(_damage);
        Destroy(gameObject);
    }
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
