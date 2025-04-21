using UnityEngine;
using UnityEngine.Serialization;

public class WeaponManager : MonoBehaviour
{
    private SpriteRenderer _toolSpriteRenderer;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private WeaponData[] _availableWeapons;
    [SerializeField] private Transform _gunHolder;
    [SerializeField] GameObject bulletPrefab;
    public WeaponData CurrentWeapon { get; private set; }
    private ToolRotator _toolRotator;
    private int _currentWeaponIndex = 0;

    void Start()
    {
        _toolRotator = GetComponentInParent<ToolRotator>();
        _toolSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        CurrentWeapon = _availableWeapons[0];
        UpdateWeaponSprite();
    }
    public void Shoot(bool isAiming = false)
    {
        if(isAiming) _toolRotator.RotateTool(isAiming);
        float angleOffset = Random.Range(-CurrentWeapon.Accuracy, CurrentWeapon.Accuracy);
        Quaternion bulletRotation = _firePoint.rotation * Quaternion.Euler(0, 0, angleOffset - 90f);
        
        GameObject bullet = Instantiate(bulletPrefab, _firePoint.position, bulletRotation);
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        bulletComponent.SetDamage(CurrentWeapon.Damage);
        bulletComponent.SetShooter(transform.parent.gameObject);
        
        
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(bullet.transform.up * CurrentWeapon.BulletSpeed, ForceMode2D.Impulse);

    }
    public void SwitchWeapon()
    {
        if (_currentWeaponIndex < _availableWeapons.Length - 1) 
            _currentWeaponIndex++;
        else
            _currentWeaponIndex = 0;
        CurrentWeapon = _availableWeapons[_currentWeaponIndex];
        UpdateWeaponSprite();
    }

    void UpdateWeaponSprite()
    {
        _toolSpriteRenderer.sprite = CurrentWeapon.WeaponSprite;
    }


    public void UpdateWeaponOrientation(int xDirection, int yDirection)
    {
        //Pistol should be moved down slightly when selected
        if (CurrentWeapon.WeaponType == WeaponData.TypeOfWeapon.Pistol) 
            _firePoint.localPosition = xDirection == -1 ? new Vector3(0.9f, -0.13f, 0) : new Vector3(0.9f, 0.13f, 0);
        
        UpdateWeaponAndGunHolderPosition(xDirection, yDirection);
    }
    public void UpdateWeaponAndGunHolderPosition(int x, int y)
    {

        Vector3 newPosition = new Vector3(x * CurrentWeapon.GunHolderOffset.x, CurrentWeapon.GunHolderOffset.y);
        
        if (y < -0.5 && CurrentWeapon.WeaponType == WeaponData.TypeOfWeapon.Pistol)
            newPosition = new Vector3(x * CurrentWeapon.GunHolderOffset.x, 0);
        
        _gunHolder.localPosition = newPosition;

    }
}