using UnityEngine;
using UnityEngine.EventSystems;
public class Shooting : MonoBehaviour
{
    [SerializeField] Transform FirePoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] private WeaponManager _weaponManager;

    private float _nextFireTime;

   
    void FixedUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject() || ItemDragHandler.IsDragging) return;
        if (!Input.GetButton("Fire1") || !(Time.time >= _nextFireTime)) return;

        Shoot();
        
        //Uses FireRate to calculate when next bullet should be fired
        _nextFireTime = Time.time + (1f/_weaponManager.CurrentWeapon.FireRate);

    }

    void Shoot()
    {
        Quaternion originalRotation = FirePoint.rotation;
     
        //Create Bullet inaccuracy depending on the current weapon 
        float angleOffset = Random.Range(-_weaponManager.CurrentWeapon.Accuracy, _weaponManager.CurrentWeapon.Accuracy);
        FirePoint.rotation *= Quaternion.Euler(0, 0, angleOffset);

        //Create bullet and apply forces + rotation
        GameObject bullet = Instantiate(bulletPrefab, FirePoint.position, FirePoint.rotation);
        //bullet.GetComponent<Bullet>().SetDamage(_weaponManager.CurrentWeapon.Damage);
        bullet.GetComponent<Bullet>().SetDamage(2);
        Debug.Log("Hit" + _weaponManager.CurrentWeapon.Damage);
        bullet.transform.rotation *= Quaternion.Euler(0, 0, -90f);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        //Add Force to the bullet in direction of fire point
        rb.AddForce(FirePoint.up * _weaponManager.CurrentWeapon.BulletSpeed, ForceMode2D.Impulse);

        //Reset the bullet rotation
        FirePoint.rotation = originalRotation;
    }


}
