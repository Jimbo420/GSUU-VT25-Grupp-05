using UnityEngine;

public class WeaponShooter : MonoBehaviour
{

    private WeaponManager _weaponManager;
    [SerializeField] GameObject bulletPrefab;
    private Transform _firePoint;
    private WeaponVisuals _weaponVisuals;
    private ToolRotator _toolRotator;

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
        if (!_weaponManager.CurrentWeapon.HasUnlimitedAmmo && _weaponManager.CurrentWeapon.CurrentAmmunition == 0) return;

        //If the player is shooting with mouse, first rotate the weapon
        if (isAiming) _toolRotator.RotateTool(isAiming);

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
    }

    public void ReloadAllWeapons(WeaponData[] weapons)
    {
        foreach (var weapon in weapons)
            weapon.CurrentAmmunition = weapon.MaxAmmunition;
    }
}
