using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

public class HealthbarBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Slider slider;
    public Image fillImage;
    public Color low;
    public Color high;

    private float _health = 0;
    private float _maxHealth = 0;
    private Dictionary<int, float> maxHealthList = new Dictionary<int, float>
    {
        {1, 25},
        {2, 40},
        {3, 60},
    };
    void Start()
    {
        SetHealth();
        slider = GetComponentInChildren<Slider>();
        fillImage = slider.fillRect.GetComponentInChildren<Image>();
    }

    public void SetHealth(int enemyLevel = 3)
    {
        _maxHealth = maxHealthList.Where(a => a.Key.Equals(enemyLevel)).Select(b => b.Value).FirstOrDefault();
        _health = _maxHealth;
        Debug.Log("Health is: " + _health);
        Health(_health, _maxHealth);
    }

    void Update()
    {
        fillImage.color = Color.Lerp(low, high, slider.normalizedValue); //Fills the slider with correct color set between low and high color
    }

    public void Health(float health, float maxhealth)
    {
        //Slider value by enemies _health
        foreach (var image in slider.GetComponentsInChildren<Image>())
            image.enabled = health != maxhealth;
        
        slider.maxValue = maxhealth;
        slider.value = health;
    }

    public void HitDamage(float damage, GameObject entity)
    {
        _health -= damage;
        Debug.Log("Damage: " + damage);
        Health(_health, _maxHealth);
        if (_health <= 0)
            Destroy(entity);
    }
}
