using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Weapon/Gun")]
public class Gun : ScriptableObject
{
    [Header("Infomation")]
    public string weaponName;

    [Header("Model")]
    public GameObject prefab;

    [Header("Bullet")]
    public GameObject bullet;
    public GameObject bulletTrail;

    [Header("Shooting")]
    public float dmg;
    public float maxDist;
    public float bulletSpeed;

    [Header("Reloading")]
    public float magSize;
    [Range(0, 1)]
    public float fireRate;
    public float reloadTime;
    public bool reloading;

}
