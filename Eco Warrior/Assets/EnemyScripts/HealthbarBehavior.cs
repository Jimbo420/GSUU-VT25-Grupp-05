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
    public Vector3 offset;
    void Start()
    {
        fillImage = slider.fillRect.GetComponentInChildren<Image>();
        slider = GetComponentInChildren<Slider>();
    }

    void Update()
    {

    }

    public void Health(float health, float maxhealth)
    {
        slider.gameObject.SetActive(health < maxhealth);
        slider.maxValue = maxhealth;
        slider.value = health;
        fillImage.color = Color.Lerp(low, high, slider.normalizedValue);
    }
}
