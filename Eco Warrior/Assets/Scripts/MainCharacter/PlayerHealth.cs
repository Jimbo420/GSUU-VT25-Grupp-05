using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private float _health = 10;
    private float _maxHealth = 10;
    [SerializeField] private PlayerHealthBarParent _healthBarParent;

    void Awake()
    {
        _health = _maxHealth;
        _healthBarParent.UpdateHearts(_health);
        Debug.Log("Nuvarande health: " + _health);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Heal()
    {
        _health = _maxHealth;
        _healthBarParent.UpdateHearts(_health);
    }
    public void HitDamage(float damage, GameObject entity)
    {
        _health -= damage;
        _health = Mathf.Clamp(_health, 0, _maxHealth);
        _healthBarParent.UpdateHearts(_health);
        if (_health <= 0)
            Destroy(entity);
        Debug.Log("Spelarens Health: " + _health);
    }
    public void Dead()
    {
        Destroy(gameObject);
    }
}
