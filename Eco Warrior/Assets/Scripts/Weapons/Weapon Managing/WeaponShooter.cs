using Assets.Scripts;
using UnityEngine;

public class WeaponShooter : MonoBehaviour
{

    private WeaponManager _weaponManager;
    [SerializeField] GameObject bulletPrefab;
    private Transform _firePoint;
    private WeaponVisuals _weaponVisuals;
    private ToolRotator _toolRotator;
    [SerializeField] private AudioSource _clipSource;
    //[SerializeField] private AudioClip _shootClip;

    void Awake()
    {
        _weaponManager = GetComponent<WeaponManager>();
        _weaponVisuals = GetComponent<WeaponVisuals>();
        _firePoint = _weaponVisuals.GetFirePoint();
        _toolRotator = _weaponVisuals.GetToolRotator();
    }
    public float GetFireRate() => _weaponManager.CurrentWeapon.FireRate;
    public void Shoot(bool isAiming = false)
    {
        bool isPlayer = transform.parent.CompareTag("Player");
        if (!isPlayer)
        {
            Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player != null)
            {
                _toolRotator.RotateToolTowards(player.position);
            }
        }
        else
        {
            if (!_weaponManager.CurrentWeapon.HasUnlimitedAmmo && _weaponManager.CurrentWeapon.CurrentAmmunition == 0) return;
            if (isAiming) _toolRotator.RotateTool(isAiming);
        }

        //If the player is shooting with mouse, first rotate the weapon

        //Calculate the direction of bullet (to create the effect of inaccuracy or spread)
        float angleOffset = Random.Range(-_weaponManager.CurrentWeapon.Accuracy, _weaponManager.CurrentWeapon.Accuracy);
        Quaternion bulletRotation = _firePoint.rotation * Quaternion.Euler(0, 0, angleOffset - 90f);

        //Create the bullet game object and assign needed values
        GameObject bullet = Instantiate(bulletPrefab, _firePoint.position, bulletRotation);
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        bulletComponent.SetDamage(_weaponManager.CurrentWeapon.Damage);
        bulletComponent.SetShooter(transform.parent.gameObject);

        //Give it a rigid body and add the force to make it move forward
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(bullet.transform.up * _weaponManager.CurrentWeapon.BulletSpeed, ForceMode2D.Impulse);
        _weaponManager.CurrentWeapon.CurrentAmmunition--;
        this.GetComponent<WeaponUI>().UpdateAmmunition();
        GetComponentInParent<SoundEmitter>().Play(_clipSource, false);

    }

    public void ReloadAllWeapons()
    {
        var weapons = _weaponManager.AvailableWeapons;
        foreach (var weapon in weapons)
            if (weapon.WeaponType is not WeaponData.TypeOfWeapon.MachineGun)
                weapon.CurrentAmmunition = weapon.MaxAmmunition;
            
        
    }

    //public void Play()
    //{
    //    footstepsSource.PlayOneShot(footstepsClip);
    //    if (transform.parent.gameObject.tag.Contains("Player"))
    //    {
    //        GetComponentInParent<SoundEmitter>().MakeSound(7f);
    //    }
    //}
}
