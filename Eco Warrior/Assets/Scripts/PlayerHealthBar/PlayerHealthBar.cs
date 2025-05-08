using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Sprite _fullHeart;
    public Sprite _halfHeart;
    public Sprite _emptyHeart;
    private Sprite _heartImage;
    //private List<int, int>();
    void Start()
    {
        _heartImage = GetComponent<Sprite>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetHeartHealth(Heart heart)
    {
        switch(heart)
        {
            case Heart.Full:
                _heartImage = _fullHeart;
                break;
            case Heart.Half:
                _heartImage = _halfHeart;
                break;
            case Heart.Empty:
                _heartImage = _emptyHeart;
                break;

        }
    }

    public enum Heart
    {
        Full = 2,
        Half = 1,
        Empty = 0
    }
}
