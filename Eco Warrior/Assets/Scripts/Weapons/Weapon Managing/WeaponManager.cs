using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private WeaponData[] _availableWeapons;
    [SerializeField] private Transform _gunHolder;
    private InventoryController _inventoryController;
    [SerializeField] private AudioSource weaponAudioSource;

    public WeaponData CurrentWeapon { get; private set; }
   
    private int _currentWeaponIndex = 0;
    
    public WeaponData[] AvailableWeapons => _availableWeapons;

    void Start()
    {
        _inventoryController = FindFirstObjectByType<InventoryController>();
        CurrentWeapon = _availableWeapons[0];
        GetComponent<WeaponShooter>().ReloadAllWeapons();
    }

    //TODO Move to Player Controller
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GetComponent<WeaponUI>().Play();
            GetComponent<WeaponShooter>().ReloadAllWeapons();
            GetComponent<WeaponUI>().UpdateAmmunition();

        }
    }

    public void EquipWeapon(int index)
    {
        List<Slot> slots = _inventoryController.GetSlots();
        if (index >= slots.Count) return;

        GameObject item = slots[index].CurrentItem;
        if (item == null) return;

        WeaponTag tag = item.GetComponent<WeaponTag>();
        if (tag == null) return;

        CurrentWeapon = tag.WeaponData;
        GetComponent<WeaponVisuals>().UpdateWeaponSprite();
        GetComponent<WeaponVisuals>().UpdateWeaponAndGunHolderPosition(1,0);
        GetComponent<WeaponUI>().UpdateAmmunition();
    }
   
   

    

    
}