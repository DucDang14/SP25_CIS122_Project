//Written by Duc Anh Dang
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static SoundManager Instance { get; set; }

    public AudioSource ShootingChanel;


    public AudioClip PistolShot;
    public AudioClip M4Shot;

    
    public AudioSource reloadingSoundM4;
    public AudioSource reloadingSoundPistol;

    public AudioSource emptyMagazineSoundPistol;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlayShootingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Pistol:
                ShootingChanel.PlayOneShot(PistolShot);
                break;
            case WeaponModel.M4:
                ShootingChanel.PlayOneShot(M4Shot);
                break;

        }
    }

    public void PlayReloadingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Pistol:
                reloadingSoundPistol.Play();
                break;
            case WeaponModel.M4:
                reloadingSoundM4.Play();
                break;

        }
    }

}
