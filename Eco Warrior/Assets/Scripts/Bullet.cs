using UnityEngine;
public class Bullet : MonoBehaviour
{
    public float maxDistance = 10f;
    private Vector3 _startPosition;
    private float _damage;

    public void SetDamage(float damage)
    {
        _damage = damage;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        EnemyMovement enemy = other.GetComponent<EnemyMovement>();
        if (enemy == null) return;

        enemy.HitDamage(_damage);
        Debug.Log("Hit" + _damage);
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
