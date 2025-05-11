using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static PlayerHealthBar;

public class PlayerHealthBarParent : MonoBehaviour
{
    public GameObject _healthBarPrefab;
    public GameObject ArmorPrefab;
    private float _armorAmount = 10f;
    public float ArmorAmount
    {
        get => _armorAmount;
        set
        {
            _armorAmount += 10;
            UpdateShield();
        }
    }
    //[SerializeField] public PlayerHealth _playerHealth;
    private List<PlayerHealthBar> _heartList = new List<PlayerHealthBar>();
    private Image[] _hearts;
    public Sprite _fullHeart;
    public Sprite _halfHeart;
    public Sprite _emptyHeart;

    private float _health = 10;
    private float _maxHealth = 10;
    private void Awake()
    {
        _health = _maxHealth;
        Debug.Log("Nuvarande health: " + _health);
        Transform[] children = GetComponentsInChildren<Transform>();
        List<Image> foundHearts = new List<Image>();
        foreach (Transform child in children)
        {
            Image heart = child.GetComponent<Image>();
            if (heart != null)
                foundHearts.Add(heart);
        }

        _hearts = foundHearts.ToArray();
        UpdateHearts(_health);
    }


    public void UpdateHearts(float currentHealth)
    {
        for (int i = 0; i < _hearts.Length; i++)
        {
            //if (_hearts[i] == null) continue;

            float heartHealth = currentHealth - (i * 2);
            Debug.Log("HeartHealth " + heartHealth);
            if (heartHealth >= 2)
                _hearts[i].sprite = _fullHeart;
            else if (heartHealth.Equals(1))
                _hearts[i].sprite = _halfHeart;
            else
                _hearts[i].sprite = _emptyHeart;
        }
    }

    public void Heal()
    {
        _health = _maxHealth;
        UpdateHearts(_health);
    }
    public void HitDamage(float damage, GameObject entity)
    {
        if (_armorAmount > 0)
        {
            RemoveShield(damage);
            return;
        }
        _health -= damage;
        _health = Mathf.Clamp(_health, 0, _maxHealth);
        UpdateHearts(_health);
        if (_health <= 0)
            Destroy(entity);
        Debug.Log("Spelarens Health: " + _health);
    }

    void UpdateShield()
    {
        int armorDisplay = Mathf.FloorToInt(_armorAmount);

        if (armorDisplay < 0)
        {
            armorDisplay = 0;
        }

        ArmorPrefab.GetComponentInChildren<TMP_Text>().text = $"{armorDisplay}";
    }
    void RemoveShield(float damage) 
    {
        _armorAmount -= damage;
        UpdateShield();
    }
    public void Dead()
    {
        Destroy(gameObject);
    }
}
