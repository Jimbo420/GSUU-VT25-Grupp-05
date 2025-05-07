using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private AudioSource _reloadSource;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.CompareTag("Ammo"))
        {
            Debug.Log("Ammo Picked up");    
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
            Debug.Log("Health Picked up");
            if (!other.gameObject.CompareTag("Player")) return;
        }
        else if (gameObject.CompareTag("Weapon"))
        {
            Debug.Log("Picked up Weapon");
        }
    }
    void Awake()
    {
        _reloadSource = GameObject.Find("Reload Audio").GetComponent<AudioSource>();
    }
}
