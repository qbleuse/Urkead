using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Enemy : MonoBehaviour
{
    [SerializeField, Range(0, 10)] private int healthPoints = 1;
    [SerializeField, Range(0f, 100f)] private float range = 10;
    [SerializeField] private bool lookAtPlayer = true;
    [SerializeField, Range(0, 1000000000)] private int scoreGained = 0;

    [SerializeField] private LonelySound soundOnDeath = null;
    [SerializeField] private ParticleSystem particleOnDeath = null;
    [SerializeField] private AnimParticleUI animOnDeath = null;


    private Transform target = null;
    private Weapon weapon = null;
    [HideInInspector] public bool        alive           = true;
    private Vector3     lookAtPlayerVec = Vector3.zero;

    public static   int         enemiesNb = 0;
    public delegate void        OnEnemyKilled();
    public static   event       OnEnemyKilled OnEnemyKilledEvent;

    public bool isAlive { get => alive; set { gameObject.SetActive(true); alive = true;  } }
    public Transform Target     { get => target;}

    /*================ EVENTS ===================*/
    private void OnTriggerEnter(Collider other)
    {
        /* Enemies loose life if touched by a player's bullet */
        if (other.CompareTag("Player Bullet"))
        {
            healthPoints--;
            if (healthPoints <= 0)
            {
                alive = false;
                gameObject.SetActive(false);
                
                OnEnemyKilledEvent?.Invoke();
                if (soundOnDeath)
                {
                    Instantiate(soundOnDeath, transform.position, transform.rotation, transform.parent);
                }
                if (particleOnDeath)
                {
                    Instantiate(particleOnDeath, transform.position, transform.rotation *  particleOnDeath.transform.rotation, transform.parent);
                };

                enemiesNb--;
                if (Player.Instance)
                {
                    Player.Instance.killScore += scoreGained;
                    Player.Instance.killedEnemyNb++;
                }

                if (enemiesNb <= 0)
                {
                    /* disable all events */
                    OnEnemyKilledEvent = null;
                }
            }
        }
    }

    /*================ UNITY METHODS ==================*/
    void Start()
    {
        enemiesNb++;

        /* Get necessary component */
        target      = GameObject.Find("Shoot Point").GetComponent<Transform>();
        weapon      = GetComponent<Weapon>();
}

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.GameIsPaused || WinScreen.gameIsWin)
            return;

        if (lookAtPlayer)
        {
            transform.LookAt(target.position);
            transform.Rotate(-transform.rotation.eulerAngles.x, 0, -transform.rotation.eulerAngles.z);
        }

        float distanceToTarget = (target.position - transform.position).magnitude;

        if (weapon && distanceToTarget < range && alive)
        {
            Vector3 targetPoint = target.position + target.forward * (distanceToTarget / 3.4f);
            weapon.firePoint.LookAt(targetPoint);
            
            weapon.Shoot();
        }
    }
}
