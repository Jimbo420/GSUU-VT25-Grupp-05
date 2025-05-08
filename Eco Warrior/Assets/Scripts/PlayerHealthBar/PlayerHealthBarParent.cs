using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerHealthBar;

public class PlayerHealthBarParent : MonoBehaviour
{
    public GameObject _healthBarPrefab;
    public PlayerHealth _playerHealth;
    private List<PlayerHealthBar> _heartList = new List<PlayerHealthBar>();
    private Image[] _hearts;
    public Sprite _fullHeart;
    public Sprite _halfHeart;
    public Sprite _emptyHeart;

    public float maxHealth = 10f;
    public float currentHealth = 10f;
    //private void Start()
    //{
    //    //InstantiateHearts();
    //    GameObject[] heartObjects = GameObject.FindGameObjectsWithTag("PlayerHearts");
    //    _hearts = new Image[heartObjects.Length];

    //    for (int i = 0; i < heartObjects.Length; i++)
    //    {
    //        _hearts[i] = heartObjects[i].GetComponent<Image>();
    //    }

    //}
    private void Awake()
    {
        Transform[] children = GetComponentsInChildren<Transform>();
        List<Image> foundHearts = new List<Image>();

        foreach (Transform child in children)
        {
            Image img = child.GetComponent<Image>();
            if (img != null)
                foundHearts.Add(img);
        }

        _hearts = foundHearts.ToArray();
    }

    public void UpdateHearts(float currentHealth)
    {
        if (_hearts == null || _hearts.Length == 0)
        {
            Debug.LogError("Hearts array is not initialized!");
            return;
        }

        for (int i = 0; i < _hearts.Length; i++)
        {
            if (_hearts[i] == null) continue; // skydda vid tomma entries

            float heartHealth = currentHealth - (i * 2);

            if (heartHealth >= 2)
                _hearts[i].sprite = _fullHeart;
            else if (heartHealth == 1)
                _hearts[i].sprite = _halfHeart;
            else
                _hearts[i].sprite = _emptyHeart;
        }
    }
    //public void InstantiateHearts()
    //{
    //    ClearHearts();
    //    float maxHealthRem = _playerHealth._maxHealth % 2;
    //    int heartsToMake = (int)((_playerHealth._maxHealth / 5) + maxHealthRem);
    //    for (int i = 0; i < heartsToMake; i++) 
    //        CreateEmptyHeart();

    //    for (int i = 0;i < _heartList.Count; i++)
    //    {
    //        int heartStatusRem = (int)Mathf.Clamp(_playerHealth._health - (i*2), 0, 2);
    //        _heartList[i].SetHeartHealth((Heart)heartStatusRem);
    //    }

    //}
    //public void CreateEmptyHeart()
    //{
    //    GameObject newHeart = Instantiate(_healthBarPrefab);
    //    newHeart.transform.SetParent(transform);
    //    PlayerHealthBar heartComponent = newHeart.GetComponent<PlayerHealthBar>();
    //    heartComponent.SetHeartHealth(Heart.Empty);
    //    _heartList.Add(heartComponent);
    //}
    //public void ClearHearts()
    //{
    //    for (int i = transform.childCount - 1; i >= 0; i--)
    //    {
    //        DestroyImmediate(transform.GetChild(i).gameObject);
    //    }
    //    _heartList.Clear();
    //}

    //public void UpdateHearts(float currentHealth)
    //{
    //    for (int i = 0; i < _heartList.Count; i++)
    //    {
    //        int heartValue = Mathf.Clamp((int)(currentHealth - (i * 2)), 0, 2);
    //        _heartList[i].SetHeartHealth((Heart)heartValue);
    //    }
    //}
}
