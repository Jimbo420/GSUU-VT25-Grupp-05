using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float maxDistance = 20f; // Maximum distance the bullet can travel
    [SerializeField] private LayerMask damageableLayers; // Layers the bullet can damage

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
        // Ignore the shooter
        if (other.gameObject == _shooter) return;
        KnockOut enemyKnockout = other.GetComponent<KnockOut>();
        if (enemyKnockout is not null && enemyKnockout.IsKnocked) return;
        // Check if the bullet hit a wall
        if (other.gameObject.layer == LayerMask.NameToLayer("Walls") ||
            other.gameObject.layer == LayerMask.NameToLayer("wall"))
        {
            Destroy(gameObject);
            return;
        }

        // Ignore objects not on the damageable layers
        if ((damageableLayers.value & (1 << other.gameObject.layer)) == 0)
        {
            Debug.Log("Not on damageable Layer");
            return;
        }

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
        if (healthbar == null) Debug.Log("Healthbar is null");
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
        // Destroy the bullet if it exceeds its maximum travel distance
        float distanceTravelled = Vector3.Distance(_startPosition, transform.position);
        if (distanceTravelled >= maxDistance)
        {
            Destroy(gameObject);
        }
    }
}
