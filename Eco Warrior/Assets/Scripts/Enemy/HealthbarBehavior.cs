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

    private float _health = 25;
    [SerializeField] private float _maxHealth = 25;
    void Start()
    {
        _health = _maxHealth;
        slider = GetComponentInChildren<Slider>();
        Health(_health, _maxHealth);
        fillImage = slider.fillRect.GetComponentInChildren<Image>();
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
