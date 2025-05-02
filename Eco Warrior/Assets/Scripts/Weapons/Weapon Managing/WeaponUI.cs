using Assets.Scripts;
using TMPro;
using UnityEngine;

public class WeaponUI : MonoBehaviour, IPlaySound
{
    [SerializeField] private TMP_Text _ammoText;
    [SerializeField] private AudioSource _reloadSource;
    [SerializeField] private AudioClip _reloadAudio;

    private WeaponManager _weaponManager;
    void Awake()
    {
        _weaponManager = GetComponent<WeaponManager>();
    }
    public void UpdateAmmunition()
    {
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
        _reloadSource.PlayOneShot(_reloadAudio);
    }
}
