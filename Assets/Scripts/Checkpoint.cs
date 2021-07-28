using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour, IComparer<Enemy>
{
    /*=============== MODIFIABLE VARIABLE ===============*/
    [Header("rotation for Transition Zone in each update")]
    [SerializeField] private    Vector3  zoneRotation        = Vector3.zero;
    [Space]
    [Header("Player's intitial rotation when penetratinfg the zone")]
    [SerializeField] private    Vector3  playerInitRotation  = Vector3.zero;
    [Space]
    [Header("speed multiplied to player in this zone")]
    [SerializeField] private    float    zoneSpeed           = 1.0f;
    [Space]
    [Header("do Input are disabled in this zone ?")]
    [SerializeField] private    bool     enableControl       = true;
    [Space]
    [Header("Do this checkpoint save the game ?")]
    [SerializeField] private    bool     enableSave          = false;
    [Space]
    [Header("=============================CamInfo=============================")]
    [SerializeField] private    Vector3  camPos = Vector3.zero;
    [Tooltip("position of the camera Relative to the player")]
    [SerializeField] private    Vector3  camRot = Vector3.zero;
    [SerializeField] private    bool     isOnRail            = false;
    [SerializeField] private    bool     lookAt              = true;


    /*============ PROPERTIES ============*/
    public Enemy[]      enemiesInZone       { get; private set;}
    public Vector3      rotation            { get => zoneRotation;}
    public Vector3      initRotation        { get => playerInitRotation; }
    public float        speed               { get => zoneSpeed; }
    public bool         control             { get => enableControl; }
    public bool         save                { get => enableSave; }
    public bool         onRail              { get => isOnRail; }
    public bool         lookAtCondition     { get => lookAt; }
    public Vector3      camPosition         { get => camPos; }
    public Vector3      camRotation         { get => camRot; }



    /*===================== EVENTS METHODS ============================*/
    public void ReturnMenu()
    {
        SceneManager.LoadScene(0);
    }

    private bool updateEnemy = false;

    void SortEnemies()
    {
        /* Look if there's any enemy in zone */
        if (enemiesInZone != null)
        {
            for (uint i = 0; i < enemiesInZone.Length; i++)
            {
                if (enemiesInZone[i] == null)
                {
                    Array.Clear(enemiesInZone, (int)i, 1);
                }
            }

            Array.Sort(enemiesInZone,Compare);
        }
    }

    void UpdateEnemy()
    {
        updateEnemy = true;
    }


    static public void OnSwitch(Checkpoint newZone)
    {
        EnemyDroneMovement[] enemyDrones = newZone.GetComponentsInChildren<EnemyDroneMovement>();

        foreach (EnemyDroneMovement enemyDrone in enemyDrones)
        {
            enemyDrone.isUp = true;
        }

        MovingObject[] movingObjects = newZone.GetComponentsInChildren<MovingObject>();

        foreach (MovingObject movingObject in movingObjects)
        {
            movingObject.ZoneEnter();
        }

        EnemyDroneMovement[] enemyDronesUps = GameObject.FindObjectsOfType<EnemyDroneMovement>();

        foreach (EnemyDroneMovement enemyDroneUp in enemyDronesUps)
        {
            if (enemyDroneUp.isUp)
                enemyDroneUp.transform.SetParent(newZone.transform);

        }
            newZone.updateEnemy = true;
    }

    /*====================== UNITY METHODS =============================*/
    // Start is called before the first frame update

    private void Awake()
    {

    }

    void Start()
    {
        enemiesInZone            = GetComponentsInChildren<Enemy>();
        Enemy.OnEnemyKilledEvent += UpdateEnemy;
        Enemy.OnEnemyKilledEvent += SortEnemies;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PauseMenu.GameIsPaused || WinScreen.gameIsWin)
            return;

        SortEnemies();
        if (updateEnemy)
        {
            updateEnemy = false;
            enemiesInZone = GetComponentsInChildren<Enemy>();
        }
        
    }

    public int Compare(Enemy x, Enemy y)
    {
        /* a destroyed GameObject is not seen in the array so at the last position */
        if (x == null || !x.isAlive || x.Target == null)
            return -1;
        else if (y == null || y.Target == null)
            return 1;
        else if (x == y)
            return 0;

        /* The sort is done looking a the distance from the player */
        float distanceToOther = (x.Target.position - y.transform.position).magnitude;
        float distanceToThis = (x.Target.position - x.transform.position).magnitude;

        /* The less far it is the more he get in the first position */
        if (distanceToOther < distanceToThis)
            return 1;
        else if (distanceToThis < distanceToOther)
            return -1;
        else
            return 0;
    }
}