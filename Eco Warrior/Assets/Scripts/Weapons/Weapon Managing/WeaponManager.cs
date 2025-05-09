using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class WeaponManager : MonoBehaviour
{
    [FormerlySerializedAs("_availableWeapons")] [SerializeField] private WeaponData[] getAvailableWeapons;
    [SerializeField] private Transform _gunHolder;
    private InventoryController _inventoryController;

    public WeaponData CurrentWeapon { get; private set; }

    private int _currentWeaponIndex = 0;
    
    public WeaponData[] GetAvailableWeapons => getAvailableWeapons;

    void Start()
    {
        _inventoryController = FindFirstObjectByType<InventoryController>();
        CurrentWeapon = getAvailableWeapons[0];
        GetComponent<WeaponShooter>().ReloadAllWeapons();
        SwitchWeapon(CurrentWeapon);
    }

    public void EquipWeapon(int index)
    {
        List<Slot> slots = _inventoryController.GetSlots();
        if (index >= slots.Count) return;

        GameObject item = slots[index].CurrentItem;
        if (item == null) return;

        WeaponTag tag = item.GetComponent<WeaponTag>();
        if (tag == null) return;
        SwitchWeapon(tag.WeaponData);
    }

    public void SwitchWeapon(WeaponData weapon)
    {
        CurrentWeapon = weapon;
        GetComponent<WeaponVisuals>().UpdateWeaponSprite();
        GetComponent<WeaponVisuals>().UpdateWeaponAndGunHolderPosition(1,0);
        GetComponent<WeaponUI>().UpdateAmmunition();
    }

    
}