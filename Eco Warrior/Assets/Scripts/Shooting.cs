using UnityEngine;
public class Shooting : MonoBehaviour
{
    [SerializeField] Transform FirePoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] private WeaponManager _weaponManager;

    private float _nextFireTime;

   
    void FixedUpdate()
    {
        if (!Input.GetButton("Fire1") || !(Time.time >= _nextFireTime)) return;

        Shoot();
        _nextFireTime = Time.time + (1f/_weaponManager.CurrentWeapon.FireRate);

    }

    void Shoot()
    {
        Quaternion originalRotation = FirePoint.rotation;
        float angleOffset = Random.Range(-_weaponManager.CurrentWeapon.Accuracy, _weaponManager.CurrentWeapon.Accuracy);
        FirePoint.rotation *= Quaternion.Euler(0, 0, angleOffset);

        GameObject bullet = Instantiate(bulletPrefab, FirePoint.position, FirePoint.rotation);
    
        bullet.transform.rotation *= Quaternion.Euler(0, 0, -90f);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(FirePoint.up * _weaponManager.CurrentWeapon.BulletSpeed, ForceMode2D.Impulse);
        FirePoint.rotation = originalRotation;
    }


}
