using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static PlayerHealthBar;

public class PlayerHealthBarParent : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject _healthBarPrefab;
    //public PlayerHealthBar _playerHealthBar; //Här ska man länka till spelarens health
    float playershealth;
    float maxhealth;
    private List<PlayerHealthBar> _heartList = new List<PlayerHealthBar>();

    public void InstantiateHearts()
    {
        ClearHearts();
        float maxHealthRem = maxhealth % 2;
        int heartsToMake = (int)((maxhealth / 2) + maxHealthRem);
        for (int i = 0; i < heartsToMake; i++) 
            CreateEmptyHeart();

        for (int i = 0;i < _heartList.Count; i++)
        {
            int heartStatusRem = (int)Mathf.Clamp(playershealth - (i*2), 0, 2);
            _heartList[i].SetHeartHealth((Heart)heartStatusRem);
        }

    }
    public void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(_healthBarPrefab);
        newHeart.transform.SetParent(transform);
        PlayerHealthBar heartComponent = newHeart.GetComponent<PlayerHealthBar>();
        heartComponent.SetHeartHealth(Heart.Empty);
        _heartList.Add(heartComponent);
    }
    public void ClearHearts()
    {
        foreach (Transform heart in transform)
        {
            Destroy(heart.gameObject);
        }
        _heartList = new List<PlayerHealthBar>();
    }
}
