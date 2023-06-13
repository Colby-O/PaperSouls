using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponManger : MonoBehaviour
{
    public Gun gunData;
    public Transform gun;

    private float reloadStart = 0.0f;
    private float lastFire = 0.0f;
    private int shotsFired = 0;

    public void Shoot()
    {
        if (shotsFired == gunData.magSize && !gunData.reloading)
        {
            reloadStart = Time.time;
            gunData.reloading = true;
        }

        if (gunData.reloading)
        {
            if (Time.time - reloadStart > gunData.reloadTime)
            {
                shotsFired = 0;
                gunData.reloading = false;
            }
        }

        if (Time.time > lastFire && !gunData.reloading)
        {
            lastFire = Time.time + gunData.fireRate;
            GameObject bulletTrail = Instantiate(gunData.bulletTrail);
            Projectile bullet = bulletTrail.AddComponent<Projectile>();
            bullet.damage = gunData.dmg;

            bulletTrail.transform.position = gun.transform.position;
            bulletTrail.transform.rotation = gun.transform.rotation;

            bulletTrail.GetComponent<Rigidbody>().AddForce(gun.forward * gunData.bulletSpeed);
            shotsFired += 1;
        }
    }

}
