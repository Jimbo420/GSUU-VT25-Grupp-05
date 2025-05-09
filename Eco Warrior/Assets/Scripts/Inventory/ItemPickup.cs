using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private AudioSource _reloadSource;
    //[SerializeField] GameObject weaponPrefab;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.CompareTag("Ammo"))
        {
            if (!other.gameObject.CompareTag("Player")) return;
            WeaponUI weaponUI = other.GetComponentInChildren<WeaponUI>();
            WeaponShooter weaponShooter= other.GetComponentInChildren<WeaponShooter>();
            GetComponentInParent<SoundEmitter>().Play(_reloadSource, false);
            weaponShooter.ReloadAllWeapons();
            weaponUI.UpdateAmmunition();
            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("Health"))
        {
            if (!other.gameObject.CompareTag("Player")) return;
            PlayerHealthBarParent healthbar = other.GetComponentInChildren<PlayerHealthBarParent>();
            if (healthbar == null) return;
            healthbar.Heal();
            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("Weapon"))
        {
            InventoryController inventory = FindFirstObjectByType<InventoryController>();
            if (inventory == null) return;

            GameObject addedItem = inventory.TryAddItem(gameObject);
            if (addedItem == null) return;

            WeaponTag weapon = addedItem.GetComponent<WeaponTag>();
            if (weapon != null && weapon.WeaponData.WeaponType == WeaponData.TypeOfWeapon.MachineGun)
                weapon.WeaponData.CurrentAmmunition = 500;
                

            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("Shield"))
        {
            
        }
    }
    void Awake()
    {
        _reloadSource = GameObject.Find("Reload Audio").GetComponent<AudioSource>();
    }
}
