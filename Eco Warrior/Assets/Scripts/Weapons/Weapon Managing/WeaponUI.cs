using TMPro;
using UnityEngine;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _ammoText;

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
}
