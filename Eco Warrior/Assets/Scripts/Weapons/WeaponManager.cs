using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponManager : MonoBehaviour
{
    private SpriteRenderer _toolSpriteRenderer;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private WeaponData[] _availableWeapons;
    [SerializeField] private Transform _gunHolder;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] private AudioSource weaponAudioSource;

    public WeaponData CurrentWeapon { get; private set; }
    private ToolRotator _toolRotator;
    private int _currentWeaponIndex = 0;

    void Start()
    {
        ReloadAmmunition();
        _toolRotator = GetComponentInParent<ToolRotator>();
        _toolSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        CurrentWeapon = _availableWeapons[0];
        UpdateWeaponSprite();
    }

    void ReloadAmmunition()
    {
        foreach (var weapon in _availableWeapons)
            weapon.CurrentAmmunition = weapon.MaxAmmunition;
    }
    public void Shoot(bool isAiming = false)
    {
        if (CurrentWeapon.CurrentAmmunition == 0) return;
        if(isAiming) _toolRotator.RotateTool(isAiming);
        float angleOffset = Random.Range(-CurrentWeapon.Accuracy, CurrentWeapon.Accuracy);
        Quaternion bulletRotation = _firePoint.rotation * Quaternion.Euler(0, 0, angleOffset - 90f);
        
        GameObject bullet = Instantiate(bulletPrefab, _firePoint.position, bulletRotation);
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        bulletComponent.SetDamage(CurrentWeapon.Damage);
        bulletComponent.SetShooter(transform.parent.gameObject);
        
        
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(bullet.transform.up * CurrentWeapon.BulletSpeed, ForceMode2D.Impulse);
        CurrentWeapon.CurrentAmmunition--;

        if (weaponAudioSource != null && CurrentWeapon.ShotSound != null)
        {
            weaponAudioSource.PlayOneShot(CurrentWeapon.ShotSound);
        }
    }
    public void SwitchWeapon()
    {
        if (_currentWeaponIndex < _availableWeapons.Length - 1) 
            _currentWeaponIndex++;
        else
            _currentWeaponIndex = 0;
        CurrentWeapon = _availableWeapons[_currentWeaponIndex];
        UpdateWeaponSprite();
        UpdateWeaponAndGunHolderPosition();
    }

    void UpdateWeaponSprite()
    {
        _toolSpriteRenderer.sprite = CurrentWeapon.WeaponSprite;
    }


    public void UpdateWeaponOrientation(float xDirection, float yDirection)
    {
        //Pistol should be moved down slightly when selected
        if (CurrentWeapon.WeaponType == WeaponData.TypeOfWeapon.Pistol)
            _firePoint.localPosition = (int)xDirection == -1 ? new Vector3(0.9f, -0.13f, 0) : new Vector3(0.9f, 0.13f, 0);
        else
            _firePoint.localPosition = new Vector3(0.9f, 0, 0);

        UpdateWeaponAndGunHolderPosition(xDirection, yDirection);
    }
    public void UpdateWeaponAndGunHolderPosition(float x = 0, float y = 0)
    {
        Vector3 newPosition;
        newPosition = Mathf.RoundToInt(x) != 0 ? new Vector3(x * CurrentWeapon.GunHolderOffset.x, CurrentWeapon.GunHolderOffset.y) : new Vector3(0, 0);

        if (y < -0.5 && CurrentWeapon.WeaponType == WeaponData.TypeOfWeapon.Pistol)
            newPosition = new Vector3(x * CurrentWeapon.GunHolderOffset.x, 0);
        
        _gunHolder.localPosition = newPosition;

    }
}