using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.CompareTag("Ammo"))
        {
            Debug.Log("Ammo Picked up");    
            if (!other.gameObject.CompareTag("Player")) return;
            WeaponUI weaponUI = other.GetComponentInChildren<WeaponUI>();
            WeaponShooter weaponShooter= other.GetComponentInChildren<WeaponShooter>();
        
            weaponShooter.ReloadAllWeapons();
            weaponUI.UpdateAmmunition();
            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("Health"))
        {
            Debug.Log("Health Picked up");
        }
        else if (gameObject.CompareTag("Weapon"))
        {
            Debug.Log("Picked up Weapon");
        }
    }
}
