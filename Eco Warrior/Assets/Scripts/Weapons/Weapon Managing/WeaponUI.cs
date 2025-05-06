using Assets.Scripts;
using TMPro;
using UnityEngine;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _ammoText;
    [SerializeField] private AudioSource _reloadSource;
    //[SerializeField] private AudioClip _reloadAudio;

    private WeaponManager _weaponManager;
    void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _ammoText = GameObject.Find("Text (TMP)").GetComponent<TMP_Text>();
        _weaponManager = GetComponent<WeaponManager>();
    }
    public void UpdateAmmunition()
    {
        Initialize();
        if (_weaponManager.CurrentWeapon.HasUnlimitedAmmo)
        {
            _ammoText.text = "\u221e/\u221e";
        }
        else
        {
            _ammoText.text = $"{_weaponManager.CurrentWeapon.CurrentAmmunition}/{_weaponManager.CurrentWeapon.MaxAmmunition}";
        }
    }

    public void Play()
    {
        GetComponentInParent<SoundEmitter>().Play(_reloadSource,  false);
    }
}
