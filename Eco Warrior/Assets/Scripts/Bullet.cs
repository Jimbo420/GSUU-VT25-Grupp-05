using UnityEngine;
public class Bullet : MonoBehaviour
{
    private float maxDistance = 20f; //Maximum of distance that the bullet can travel
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
        
        HealthbarBehavior healthbar = other.GetComponentInChildren<HealthbarBehavior>();
        if (healthbar == null)
        {
            Debug.Log("Health is null");
            return;
        }

        healthbar.HitDamage(_damage, other.gameObject);
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
