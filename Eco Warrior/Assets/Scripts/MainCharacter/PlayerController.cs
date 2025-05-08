using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private WeaponShooter _weaponShooter;
    private WeaponManager _weaponManager;
    private float _nextFireTime;
    private AudioSource _footstepsSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _weaponManager = GetComponentInChildren<WeaponManager>();
        _weaponShooter = GetComponentInChildren<WeaponShooter>();
        _footstepsSource = GameObject.Find("Step Audio").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            EquipWeapon(2);


    }

    void EquipWeapon(int index)
    {
        _weaponManager.EquipWeapon(index);
    }
    void MakeSound()
    {
            Debug.Log("Making sound");
            GetComponent<SoundEmitter>().Play(_footstepsSource, true);

    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            MakeSound();
            
        }
        //if (EventSystem.current.IsPointerOverGameObject() || ItemDragHandler.IsDragging) return;
        if (!Input.GetButton("Fire1") || !(Time.time >= _nextFireTime)) return;
        
        _weaponShooter.Shoot(true);
        
        //Uses FireRate to calculate when next bullet should be fired
        _nextFireTime = Time.time + (1f/_weaponShooter.GetFireRate());
    }
    public void Stop()
    {
        _footstepsSource.Stop();
    }
}
