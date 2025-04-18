//Written by Duc Anh Dang
//03/23/2025
using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;
    public int weaponDamage;

    //Shooting 
    [Header("Shooting")]
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 0.5f;

    //Burst
    [Header("Burst")]
    public int bulletsPerBurst = 3;
    public int burstBulletLeft;

    [Header("Spread")]
    //Spread
    public float spreadIntensity;
    public float hipSpreadIntensity;
    public float adsSpreadIntensity;


    //origin
    //Bullet 
    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 500f;
    public float bulletPrefabLifeTime = 3f;

    public GameObject muzzleEffect;
    internal Animator animator;


    //Loading
    [Header("Loading")]
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    bool isADS;

    public enum WeaponModel
    {
        Pistol,
        M4,
        LaserGun
    }
    public WeaponModel thisWeaponModel;


    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    public ShootingMode currentShootingMode;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();

        //Loading
        bulletsLeft = magazineSize;

        spreadIntensity = hipSpreadIntensity;

    }

    // Update is called once per frame
    void Update()
    {
        //empty sound play
        if (isActiveWeapon)
        {
            if (Input.GetMouseButtonDown(1))
            {
                EnterADS();
            }

            if (Input.GetMouseButtonUp(1))
            {
                ExitADS();

            }



            GetComponent<Outline>().enabled = false;

            if (bulletsLeft == 0 && isShooting)
            {
                SoundManager.Instance.emptyMagazineSoundPistol.Play();
            }


            if (currentShootingMode == ShootingMode.Auto)
            {
                //Holding Down Left Mouse Button
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else if (currentShootingMode == ShootingMode.Single ||
                    currentShootingMode == ShootingMode.Burst)
            {
                //Clicking Left Mouse Button Once
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0)
            {
                Reload();
            }

            //Automatically reload when magazine is empty
            if (readyToShoot && isShooting == false && isReloading == false && bulletsLeft <= 0)
            {
                //    Reload();
            }

            if (readyToShoot && isShooting && bulletsLeft > 0)
            {
                burstBulletLeft = bulletsPerBurst;
                FireWeapon();
            }


        }


    }
    private void EnterADS()
    {
        animator.SetTrigger("enterADS");
        isADS = true;
        HUDManager.Instance.middleDot.SetActive(false);
        spreadIntensity = adsSpreadIntensity;
    }
    private void ExitADS()
    {
        animator.SetTrigger("exitADS");
        isADS = false;
        HUDManager.Instance.middleDot.SetActive(true);
        spreadIntensity = hipSpreadIntensity;
    }


    private void FireWeapon()
    {
        bulletsLeft--;


        muzzleEffect.GetComponent<ParticleSystem>().Play();

        if (isADS)
        {
            animator.SetTrigger("RECOIL_ADS");
        }
        else
        {
            animator.SetTrigger("RECOIL");

        }


        //SoundManager.Instance.shootingSoundPistol.Play();

        SoundManager.Instance.PlayShootingSound(thisWeaponModel);


        readyToShoot = false;
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        //Instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        Bullet bul = bullet.GetComponent<Bullet>();
        bul.bulletDamage = weaponDamage;


        //Pointing the bullet to face the shooting direction
        bullet.transform.forward = shootingDirection;

        //Shot the bullet
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        //Destroy the bullet after some time
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
        //checking if we are done shooting
        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }
        //Burst Mode
        if (currentShootingMode == ShootingMode.Burst && burstBulletLeft > 1) //We already shoot once before this check 
        {
            burstBulletLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    //Reloead
    private void Reload()
    {

        SoundManager.Instance.PlayReloadingSound(thisWeaponModel);

        animator.SetTrigger("RELOAD");

        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);

    }

    private void ReloadCompleted()
    {
        if (WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > magazineSize)
        {
            bulletsLeft = magazineSize;
            WeaponManager.Instance.DecreaseTotalAmmo(bulletsLeft, thisWeaponModel);
        }
        else
        {
            bulletsLeft = WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel);
            WeaponManager.Instance.DecreaseTotalAmmo(bulletsLeft, thisWeaponModel);
        }




        isReloading = false;
    }


    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }
    public Vector3 CalculateDirectionAndSpread()
    {
        // Shooting from the middle of the screen to check where are we pointing at
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            //Hitting Something
            targetPoint = hit.point;
        }
        else
        {
            //Shooting at the air
            targetPoint = ray.GetPoint(100);
        }
        Vector3 direction = targetPoint - bulletSpawn.position;

        float z = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        //Returning the shooting direction and spread
        return direction + new Vector3(0, y, z);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}


