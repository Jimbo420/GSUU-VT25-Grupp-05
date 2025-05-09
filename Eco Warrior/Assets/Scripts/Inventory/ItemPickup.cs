using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private AudioSource _reloadSource;
    //[SerializeField] GameObject weaponPrefab;
    void OnTriggerEnter2D(Collider2D other)
    {
            if (!other.gameObject.CompareTag("Player")) return;
        if (gameObject.CompareTag("Ammo"))
        {
            WeaponUI weaponUI = other.GetComponentInChildren<WeaponUI>();
            WeaponShooter weaponShooter= other.GetComponentInChildren<WeaponShooter>();
            GetComponentInParent<SoundEmitter>().Play(_reloadSource, false);
            weaponShooter.ReloadAllWeapons();
            weaponUI.UpdateAmmunition();
            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("Health"))
        {
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
            PlayerHealthBarParent health = other.GetComponentInChildren<PlayerHealthBarParent>();
            health.ArmorAmount += 10;
            Destroy(gameObject);
        }
    }
    void Awake()
    {
        _reloadSource = GameObject.Find("Reload Audio").GetComponent<AudioSource>();
    }
}
