using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private float _health = 25;
    private float _maxHealth = 25;
    void Start()
    {
        _health = _maxHealth;
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
        Health(_health, _maxHealth);
    }
    public void HitDamage(float damage, GameObject entity)
    {
        _health -= damage;
        //Debug.Log("Damage: " + damage);
        Health(_health, _maxHealth);
        if (_health <= 0)
            Destroy(entity);
    }
}
