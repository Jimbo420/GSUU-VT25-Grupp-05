using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] Sprite _fullHeart;
    [SerializeField] Sprite _halfHeart;
    [SerializeField] Sprite _emptyHeart;
    [SerializeField] private List<Sprite> heartImages;
    //private List<int, int>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayersHearts(10);
        Debug.Log("Hej");
    }

    public void UpdatePlayersHearts(int health)
    {
        for (int i = 0; i < heartImages.Count; i++)
        {
            if (health >= (i + 1) * 2)
                heartImages[i] = _fullHeart;
            else if (health == (i * 2) + 1)
                heartImages[i] = _halfHeart;
            else
                heartImages[i] = _emptyHeart;
        }
    }
}
