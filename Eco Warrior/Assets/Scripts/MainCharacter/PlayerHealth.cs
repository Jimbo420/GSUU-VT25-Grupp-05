using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float _health = 25;
    public float _maxHealth = 25;
    [SerializeField] private PlayerHealthBarParent _healthBarParent;

    void Start()
    {
        _health = _maxHealth;
        _healthBarParent.UpdateHearts(_health);
        Debug.Log("Nuvarande health: " + _health);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Health(float health, float maxhealth)
    {
        //Slider value by enemies _health
        //foreach (var image in slider.GetComponentsInChildren<Image>())
        //    image.enabled = health != maxhealth;

        //slider.maxValue = maxhealth;
        //slider.value = health;
    }
    public void Heal()
    {
        _health = _maxHealth;
        _healthBarParent.UpdateHearts(_health);
        Health(_health, _maxHealth);
    }
    public void HitDamage(float damage, GameObject entity)
    {
        _health -= damage;
        _health = Mathf.Clamp(_health, 0, _maxHealth);
        _healthBarParent.UpdateHearts(_health);
        Health(_health, _maxHealth);
        if (_health <= 0)
            Destroy(entity);
        //else
            //_healthBarParent.UpdateHearts(_health);

        Debug.Log("Spelarens Health: " + _health);
    }
    public void Dead()
    {
        Destroy(gameObject);
    }
}
