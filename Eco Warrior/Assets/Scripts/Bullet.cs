using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float maxDistance = 20f; // Maximum distance the bullet can travel
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
        if (other.gameObject == _shooter) return; // Ignore the shooter

        // Check if the target implements IDamageable
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(_damage);
            Destroy(gameObject);
            return;
        }

        // Fall back to HealthbarBehavior for compatibility
        HealthbarBehavior healthbar = other.GetComponentInChildren<HealthbarBehavior>();
        if (healthbar != null)
        {
            healthbar.HitDamage(_damage, other.gameObject);
            Destroy(gameObject);
            return;
        }

        // If no damage system is found, log a warning
        Debug.LogWarning($"Bullet hit {other.gameObject.name}, but it cannot take damage.");
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




