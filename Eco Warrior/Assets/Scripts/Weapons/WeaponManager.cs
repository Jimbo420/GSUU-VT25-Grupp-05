using UnityEngine;
using UnityEngine.Serialization;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _toolSpriteRenderer;
    [SerializeField] private Transform _firePoint;
    public WeaponData CurrentWeapon { get; private set; }
    [SerializeField] private WeaponData[] _availableWeapons;
    [SerializeField] private Transform _gunHolder;
    private int _currentWeaponIndex = 0;
    [SerializeField] GameObject bulletPrefab;

    void Start()
    {
        CurrentWeapon = _availableWeapons[0];
        UpdateWeaponSprite();
    }
    public void Shoot()
    {
        
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
        UpdateFirePointPosition();
    }

    void UpdateWeaponSprite()
    {
        _toolSpriteRenderer.sprite = CurrentWeapon.WeaponSprite;
    }

    void UpdateFirePointPosition()
    {
        //Updates from where the shot is fired
        _firePoint.localPosition = new Vector3(0, CurrentWeapon.FirePointOffset.y, 0);
    }

    public void UpdateWeaponOrientation(int xDirection, int yDirection)
    {
        //Horizontal Movement
        if (xDirection is 1 or -1)
        {
            //Pistol should be moved down slightly when selected
            if (CurrentWeapon.WeaponType == WeaponData.TypeOfWeapon.Pistol)
                _firePoint.localPosition = new Vector3(0, -0.13f, 0); 
            
        }
        //Vertical Movement
        else if(yDirection is 1 or -1)
            _firePoint.localPosition = new Vector3(0, 0, 0);

        
        UpdateWeaponAndGunHolderPosition(xDirection, yDirection);
    }
    public void UpdateWeaponAndGunHolderPosition(int x, int y)
    {
        Vector3 newPosition = new Vector3();

        // Diagonal
        if (x != 0 && y != 0)
        {
            float offsetX = 0.65f;
            float offsetY = y > 0 ? 0.4f : -0.4f;
            newPosition = new Vector3(x * offsetX, offsetY);
        }
        // Horizontal
        else if (x != 0)
        {
            newPosition = new Vector3(x * CurrentWeapon.GunHolderOffset.x, CurrentWeapon.GunHolderOffset.y);
        }
        // Vertical
        else if (y != 0)
        {
            newPosition = new Vector3(0, y * 0.9f);
        }


        _gunHolder.localPosition = newPosition;



    }
}