using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [SerializeField]                        public  Transform   firePoint                   = null;
    [SerializeField]                        private GameObject  bulletPrefab                = null;
    [SerializeField, Range(0.0f, 5.0f)]     private float       shootCooldown               = 0.0f;
    [SerializeField]                        private bool        limitedBullet               = true;
    [SerializeField, Range(0, 100)]         public  int         bulletNb                    = 60;
    [SerializeField]                        private bool        burstShot                   = false;
    [SerializeField]                        private int         nbBulletsInBurst            = 0;
    [SerializeField]                        private int         framesBetweenBulletsInBurst = 3;
    
    private AudioSource shotSound = null;

    private float   cooldown;

    public delegate void        OnShoot(int _bulletNb);
    public event    OnShoot     OnShootEvent;
    public bool     LimitedBullet { get => limitedBullet; }

    private int currentBulletsInBurst;
    private int currentFrameInBurst;


    /*===================== UNITY METHODS =======================*/
    void Start()
    {
        cooldown = 0;
        if (burstShot)
        {
            currentBulletsInBurst = nbBulletsInBurst;
            currentFrameInBurst = framesBetweenBulletsInBurst;
        }
        shotSound = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        cooldown -= Time.deltaTime;
    }


    /*========================== EVENTS ==============================*/
    private void OnDestroy()
    {
        /* clear all events*/
        OnShootEvent = null;
    }

    /*============================ MAIN METHODS ================================*/
    public void Shoot()
    {
        if (PauseMenu.GameIsPaused || WinScreen.gameIsWin)
            return;

        if ((bulletNb > 0 || !limitedBullet) && cooldown < 0 && firePoint != null && bulletPrefab != null)
        {
            if (burstShot)
            {
                currentFrameInBurst--;
                if (currentFrameInBurst == 0)
                {
                    currentBulletsInBurst--;
                    currentFrameInBurst = framesBetweenBulletsInBurst;
                    shotSound?.PlayOneShot(shotSound.clip);
                    Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                    bulletNb--;

                    if (limitedBullet)
                        OnShootEvent?.Invoke(bulletNb);
                    else
                        OnShootEvent?.Invoke(int.MaxValue);

                    if (currentBulletsInBurst == 0)
                    {
                        cooldown = shootCooldown;
                        currentBulletsInBurst = nbBulletsInBurst;
                    }
                }
            }

            else
            {
                cooldown = shootCooldown;
                shotSound?.PlayOneShot(shotSound.clip);
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                bulletNb--;
                if (limitedBullet)
                    OnShootEvent?.Invoke(bulletNb);
                else
                    OnShootEvent?.Invoke(int.MaxValue);
            }
        }
    }
}
