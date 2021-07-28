using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public struct PlayerSave
{
    public Vector3  position;
    public float    speed;
    public int      bullet;
    public float    zLock;
    public int      killScore;
}

[RequireComponent(typeof(MeshRenderer), typeof(CharacterController),typeof(Animator))]
public class Player : MonoBehaviour, IComparer<Enemy>
{
    [Header("===============SpeedInfo==============")]
    [Space]
    [SerializeField, Range(0.0f, 10.0f)]    public  float       speed                   = 6.2f;
    [SerializeField, Range(0.0f, 10.0f)]    private float       minSpeed                = 3.0f;
    [SerializeField, Range(0.0f, 10.0f)]    private float       maxSpeed                = 6.2f;
    [SerializeField, Range(0.0f, 3.0f)]     private float       speedAccel              = 0.3f;
    [SerializeField, Range(0.0f, 3.0f)]     private float       speedSlow               = 0.3f;
    [SerializeField, Range(0.0f, 3.0f)]     public  float       speedSlowShot           = 0.3f;
    [SerializeField, Range(0.0f, 3.0f)]     private float       speedSlowZone           = 0.3f;
    [SerializeField, Range(0.0f, 3.0f)]     private float       speedSlowBox            = 0.3f;
    [Space]
    [Header("===============JumpInfo===============")]
    [Space]
    [SerializeField, Range(0.0f, 100.0f)]   private float       gravityScale            = 15.0f;
    [SerializeField, Range(0.0f, 100.0f)]   private float       jumpSpeed               = 8f;
    [SerializeField, Range(0.0f, 1.0f)]     private float       jumpDurationThreshold   = 0.25f;
    [SerializeField, Range(0.0f, 60.0f)]    private float       upCoeff                 = 10.0f;
    [SerializeField, Range(0.0f, 60.0f)]    private float       downCoeff               = 10.0f;
    [Space]
    [Header("===============SlideInfo===============")]
    [Space]
    [SerializeField]                        private Vector2     SlideControllerInfo     = Vector2.zero;
    [Header("X is the radius of the capsule, Y is the height")]
    [SerializeField, Range(0.0f, 2.0f)]     private float       slideDuration           = 0.5f;
    [Space]
    [Header("===============AimInfo===============")]
    [Space]
    [SerializeField, Range(0.0f, 90.0f)]    private float       coneAngle               = 5.0f;
    [SerializeField]                        private GameObject  enemiesInDepth          = null;
    [SerializeField, Range(0.0f, 100.0f)]   private float       range                   = 50.0f;
    [SerializeField]                        private RawImage    targetImage             = null;
    [Space]
    [Header("===============DashInfo===============")]
    [Space]
    [SerializeField, Range(0, 5)]           private int             dashMaxNb           = 3;
    [SerializeField, Range(0.0f, 2.0f)]     private float           dashDuration        = 0.5f;
    [SerializeField, Range(0.0f, 3.0f)]     private float           dashCooldown        = 1.0f;
    [SerializeField, Range(0.0f, 3.0f)]     private float           dashReloadCooldown  = 2.0f;
    [SerializeField, Range(0.0f, 100.0f)]   private float           dashVelocity        = 25.0f;
    [SerializeField]                        private ParticleSystem  dashParticles       = null;
    [SerializeField]                        private LonelySound     dashAudio           = null;
    [SerializeField]                        private LonelySound     dashReloadAudio     = null;
    [Space]
    [Header("===============ClimbInfo===============")]
    [Space]
    [SerializeField, Range(0.0f, 100.0f)]   private float       climbSpeed              = 6f;
    [Space]
    [Header("===============ScoringSystem===============")]
    [Space]
    [SerializeField, Range(0, 1000000000)]  private int         startScore          = 0;
    [Space]
    [Header("from 0 to the equivalent of 5min in sec")]
    [SerializeField, Range(0.0f, 300.0f)]   private float       timeForLossOfPoint  = 60.0f; 
    [Space]
    [SerializeField, Range(0, 1000000000)]  private int         pointLostPerSec     = 0;
    [SerializeField, Range(0, 1000000000)]  private int         pointByAmmo         = 0;
    [SerializeField, Range(0, 1000000000)]  private int         pointBySpeed        = 0;

    [HideInInspector] private static Player         instance        = null;
    public static Player Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
            return instance;
        }
    }

    [HideInInspector] public        int             finalScore      = 0;

    [HideInInspector] public        int             killScore       = 0;
    [HideInInspector] public        int             bonusScore      = 0;
    [HideInInspector] public        int             timeScore       = 0;
    [HideInInspector] public        int             killedEnemyNb   = 0;
    [HideInInspector] public        PlayerCamera    playerCam       = null;
    [HideInInspector] public        float           timer           = 0.0f;
    [HideInInspector] public        ParticleSystem  enemyDeath      = null;
    [HideInInspector] public        bool            updateEnemy     = false;
    [HideInInspector] public        Weapon          weapon          = null;

    private bool                isClimbing              = false;
    private bool                isSliding               = false;
    private bool                isJumping               = false;
    private int                 dashNb                  = 0;
    private Enemy               targetEnemy             = null;
    private float               jump                    = 0.0f;
    private Checkpoint          currentZoneTransform    = null;
    private Vector2             rightJoystick           = Vector2.zero;
    private Vector2             dashDir                 = Vector2.zero;
    private Vector2             aimDir                  = Vector2.zero;
    private Vector3             lockDir                 = Vector3.zero;
    private Vector2             triggerState            = Vector2.zero;
    private Vector2             controllerInfo          = Vector3.zero;
    private float               cooldown                = 0.0f;
    private float               reloadCooldown          = 0.0f;
    private float               rayCastLengthCheck      = 0.6f;
    private float               jumpDuration            = 0.0f;
    private float               dashDur                 = 0.0f;
    private float               slideDur                = 0.0f;
    private float               zLock                   = 0.0f;
    private CharacterController controller              = null;
    private Enemy[]             depthEnemies            = null;
    private Animator            animator                = null;
    private Transform           rotatePoint             = null;
    private Transform           preshotPoint            = null;
    private RectTransform       uiRectTransform         = null;
    private PushInfo            pushInfo;
    private PlayerSave          playerSave;
    public  PlayerSave          PlayerSave{ get => playerSave; }
    public  float               MinSpeed{ get => minSpeed; }
    public  float               MaxSpeed{ get => maxSpeed; }
    
    public delegate void    OnZoneSwitch(Checkpoint checkpoint);
    public static   event   OnZoneSwitch OnZoneSwitchEvent;

    private void loadSave()
    {
        dashParticles.gameObject.SetActive(false);
        controller.enabled = false;
        GameMgr.LoadSave();
        controller.enabled = true;
        dashParticles.gameObject.SetActive(true);
    }

    /*======================= ANIMATION ============================*/

    private void LateUpdate()
    {
        if (targetEnemy && !targetEnemy.isAlive && !isSliding && !isClimbing)
        {
            rotatePoint.LookAt(targetEnemy.transform);
            rotatePoint.rotation *= Quaternion.Euler(0.0f, -90.0f, -83.0f - rotatePoint.rotation.z);
        }
    }

    /*======================= ENEMIES ==========================*/
    /* when Zone Changes Enemies are sort depending on their distance from the player*/
    void SortEnemies()
    {
        /* Look if there's any enemy in depth */
        if (depthEnemies != null)
        {
            for (uint i = 0; i < depthEnemies.Length; i++)
            {
                if (depthEnemies[i] == null)
                {
                    Array.Clear(depthEnemies, (int)i, 1);
                }
            }
            
            /* BUG UNABLE TO FIX BEFORE GOLD */
            //Array.Sort(depthEnemies,Compare);
        }
    }

    public int Compare(Enemy x, Enemy y)
    {
        /* a destroyed GameObject is not seen in the array so at the last position */
        if (x == null || !x.isAlive)
            return -1;
        else if (y == null)
            return 1;
        else if (x == y || x.Target == null || y.Target == null)
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

    private void EnemyKilled()
    {
        speed += speedAccel;
        UpdatePreshotPoint();
        if (playerCam)
            playerCam.MakeShake(0.1f,0.05f);
    }

    /* Called when an ennemy is killed to update the Enemy Array*/
    void UpdateEnemy()
    {
        updateEnemy = true;
    }

    public void UpdatePreshotPoint()
    {
        if (preshotPoint)
        {
            preshotPoint.Translate(-preshotPoint.localPosition);
            preshotPoint.Translate(Vector3.forward * speed * 0.14f + Vector3.up * 1.3f);
        }
    }

    /*============= CHECKPOINT =============*/
    /* for changing direction in zones*/
    private void OnTriggerEnter(Collider collider)
    {

        if (collider.gameObject.CompareTag("Checkpoint"))
            OnZoneSwitchEvent?.Invoke(collider.gameObject.GetComponent<Checkpoint>());

        if (collider.gameObject.CompareTag("Kill Zone"))
            loadSave();
        if (collider.gameObject.CompareTag("Slow Box"))
            speed -= speedSlowBox;
    }

    private void OnDestroy()
    {
        instance            = null;
        OnZoneSwitchEvent   = null;
        Array               .Clear(depthEnemies, 0, depthEnemies.Length);
        depthEnemies        = null;
        preshotPoint        = null;
        if (playerCam)
        {
            playerCam       .StopAllCoroutines();
            playerCam       = null;
        }
    }

    /*============= CLIMBING =============*/
    private void OnTriggerStay(Collider other)
    {
        /* Climbing check */
        if (other.CompareTag("climbable"))
            isClimbing = true;
        else if (other.CompareTag("Slow Zone"))
        {
            speed -= speedSlow;
            if (speed < minSpeed)
                speed = 0;
            UpdatePreshotPoint();
        }
        else if (other.CompareTag("PushZone"))
        {
            pushInfo = other.gameObject.GetComponent<PushZone>().pushZoneInfo;
        }
        else
        {
            pushInfo.pushedCoeff     = 0f;
            pushInfo.pushedDirection = Vector3.zero;
            isClimbing = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("climbable"))
            isClimbing = false;
        else if (other.CompareTag("PushZone"))
        {
            pushInfo.pushedCoeff     = 0f;
            pushInfo.pushedDirection = Vector3.zero;
        }
    }

    /*============ SLIDING ==============*/
    /* changes the controller's radius and height depending on the animation/position of the player */
    private void ChangeControllerInfo(Vector2 newControllerInfo)
    {
        controller.radius           = newControllerInfo.x;
        controller.height           = newControllerInfo.y;
    }

    /*=========================== AIMING ================================*/

    /* gives a unit vector corresponding to the direction of a first pos
       to a second in screen coordinates */
    private Vector3 DirOnScreen(Vector3 firstPos, Vector3 secondPos)
    {
        Vector3 firstPosPlane      = playerCam.cam.WorldToScreenPoint(firstPos);
        Vector3 secondPosPlane     = playerCam.cam.WorldToScreenPoint(secondPos);
        firstPosPlane.x            -= (float)playerCam.cam.pixelWidth    / 2.0f;
        firstPosPlane.y            -= (float)playerCam.cam.pixelHeight   / 2.0f;
        secondPosPlane.x           -= (float)playerCam.cam.pixelWidth    / 2.0f;
        secondPosPlane.y           -= (float)playerCam.cam.pixelHeight   / 2.0f;

        return (secondPosPlane - firstPosPlane);
    }

    /* Used to aim on an enemy depending on the direction given by the joystick (on an other plane than his)*/
    private void AimInDepth()
    {
        /* Look if there's any enemy in depth */
        if (depthEnemies != null)
        {
            Debug.DrawRay(weapon.firePoint.position, playerCam.cam.transform.TransformDirection(aimDir), Color.green);
            /* look for all enemies*/
            foreach (Enemy enemy in depthEnemies)
            {
                if (enemy == null)
                    continue;

                if ((transform.position - enemy.transform.position).magnitude >= range)
                {
                    targetEnemy = null;
                    continue;
                }

                Vector3 dirEnemyPlayer = playerCam.cam.transform.TransformDirection(DirOnScreen(weapon.firePoint.position, enemy.transform.position));

                aimDir                  = playerCam.cam.transform.TransformDirection(aimDir);
                float angle = Vector3   .Angle(dirEnemyPlayer.normalized, aimDir);

                if (angle <= coneAngle)
                {
                    targetEnemy = enemy;
                    break;
                }
                else
                    targetEnemy = null;
            }

            if (targetEnemy != null)
            {
                float aimInHead = 0;

                if (!targetEnemy.GetComponent<EnemyDroneMovement>())
                    aimInHead = transform.transform.localScale.y / 2.0f;

                lockDir = (targetEnemy.transform.position + targetEnemy.transform.up * aimInHead) - weapon.firePoint.position;
                Debug.DrawRay(weapon.firePoint.position, lockDir, Color.yellow);
                weapon.firePoint.LookAt(weapon.firePoint.position + lockDir);
                weapon.firePoint.Rotate(90, 0, 0);
            }
            else
            {
                weapon.firePoint.LookAt(weapon.transform.position + playerCam.cam.transform.TransformDirection(aimDir));
                weapon.firePoint.Rotate(90, 0, 0);
            }
        }
    }

    /* used to aim on an enemy depending on the direction given by the joystick (on the same plane than his)*/
    private void Aim()
    {
        if (currentZoneTransform != null && currentZoneTransform.enemiesInZone != null)
        {
            Debug.DrawRay(weapon.firePoint.position, playerCam.cam.transform.TransformDirection(aimDir), Color.green);
            foreach (Enemy enemy in currentZoneTransform.enemiesInZone)
            {
                if (enemy == null)
                    continue;

                if ((transform.position - enemy.transform.position).magnitude >= range)
                {
                    targetEnemy = null;
                    continue;
                }

                Vector3 dirEnemyPlayer  = playerCam.cam.transform.TransformDirection(DirOnScreen(weapon.firePoint.position, enemy.transform.position));



                aimDir                  = playerCam.cam.transform.TransformDirection(aimDir);
                float angle             = Vector3.Angle(dirEnemyPlayer.normalized, aimDir);

                if (angle <= coneAngle)
                {
                    targetEnemy = enemy;
                    break;

                }
                else
                    targetEnemy = null;
            }

            if (targetEnemy != null)
            {
                float aimInHead = 0;

                if (!targetEnemy.GetComponent<EnemyDroneMovement>())
                    aimInHead = transform.transform.localScale.y / 2.0f;

                lockDir = (targetEnemy.transform.position + targetEnemy.transform.up * aimInHead) - weapon.firePoint.position;
                Debug.DrawRay(weapon.firePoint.position, lockDir, Color.yellow);
                weapon.firePoint.LookAt(weapon.firePoint.position + lockDir);
                weapon.firePoint.Rotate(90, 0, 0);

            }
            else
            {
                weapon.firePoint.LookAt(weapon.transform.position + playerCam.cam.transform.TransformDirection(aimDir));
                weapon.firePoint.Rotate(90, 0, 0);
            }
        }
    }


    /*============================ UNITY FUNCTION ==============================*/

    private void Awake()
    {

    }

    /* Start is called before the first frame update */
    void Start()
    {
        instance            = GetComponent<Player>();
        animator            = GetComponent<Animator>();
        rotatePoint         = animator.GetBoneTransform(HumanBodyBones.Spine);
        controller          = GetComponent<CharacterController>();
        playerCam           = GameObject.Find("Main Camera").GetComponent<PlayerCamera>();
        weapon              = GetComponentInChildren<Weapon>();
        if (targetImage)
            uiRectTransform     = targetImage.canvas.GetComponent<RectTransform>();
        OnZoneSwitchEvent   = (Checkpoint newZone) =>
        {
            if (newZone != null)
            {
                currentZoneTransform = newZone;
                transform.Rotate(-transform.rotation.eulerAngles);
                transform.Rotate(currentZoneTransform.initRotation);

                if (newZone.gameObject.name.StartsWith("End"))
                {
                    WinScreen.gameIsWin = true;
                    bonusScore += (weapon.bulletNb * pointByAmmo);
                    finalScore += bonusScore;
                    finalScore += killScore;
                    finalScore += timeScore;
                    TimeSave.SaveTimer();
                }

                if (newZone.save)
                {
                    playerSave.position     = transform.position;
                    playerSave.speed        = speed;
                    playerSave.bullet       = weapon.bulletNb;
                    playerSave.zLock        = zLock;
                    playerSave.killScore    = killScore;
                }

                if (currentZoneTransform.rotation.sqrMagnitude == 0.0f)
                    zLock = transform.position.z;
            }
        };

        playerSave.position = transform.position;
        playerSave.speed    = speed;
        playerSave.bullet   = weapon.bulletNb;
        playerSave.zLock    = transform.position.z;

        finalScore          = startScore;
        killedEnemyNb       = 0;

        OnZoneSwitchEvent += playerCam.CamChangeInfo;
        OnZoneSwitchEvent += Checkpoint.OnSwitch;
        OnZoneSwitchEvent += GameMgr.Instance.UpdateSave;

        if (enemiesInDepth != null)
        {
            Enemy.OnEnemyKilledEvent += UpdateEnemy;
            Enemy.OnEnemyKilledEvent += SortEnemies;
            depthEnemies             = enemiesInDepth.GetComponentsInChildren<Enemy>();
        }

        Enemy.OnEnemyKilledEvent += EnemyKilled;
        preshotPoint = GameObject.Find("Shoot Point").transform;
        
   

        dashNb           = dashMaxNb;
        controllerInfo.x = controller.radius;
        controllerInfo.y = controller.height;
        GameMgr.Instance.SetMaxSpeed(maxSpeed);
        GameMgr.Instance.SetMinSpeed(minSpeed);

        pushInfo.pushedDirection = Vector3.zero;
        pushInfo.pushedCoeff     = 0f;

        reloadCooldown = dashReloadCooldown;

    }


    private float pointTimer = 0.0f;
    /* Update is called once per frame */
    void Update()
    {
        if (PauseMenu.GameIsPaused || WinScreen.gameIsWin || TrainingRead.Read && animator)
        {
            animator.enabled = false;
            return;
        }
        else if (animator)
            animator.enabled = true;

        SortEnemies();
        if (speed <= minSpeed)
            loadSave();

        /* getting Inputs and see if it can*/
        if (PauseMenu.GameIsPaused || WinScreen.gameIsWin || TrainingRead.Read)
        {
            jump            = 0.0f;
            rightJoystick.x = 0.0f;
            rightJoystick.y = 0.0f;
            aimDir.x        = 0.0f;
            aimDir.y        = 0.0f;
            triggerState.x  = -1f;
            triggerState.y  = -1f;
        }
        else if (currentZoneTransform != null && currentZoneTransform.control && GameMgr.controllerType)
        {
            jump            = Input.GetAxis("Jump");
            rightJoystick.x = Input.GetAxis("RightJoystickHorizontal");
            rightJoystick.y = Input.GetAxis("RightJoystickVertical");
            aimDir.x        = Input.GetAxis("LeftJoystickHorizontal");
            aimDir.y        = Input.GetAxis("LeftJoystickVertical");
            triggerState.x  = Input.GetAxis("LeftTrigger");
            triggerState.y  = Input.GetAxis("RightTrigger");
        }
        else if (currentZoneTransform != null && currentZoneTransform.control && !GameMgr.controllerType)
        {
            jump            = Input.GetAxis("XboxJump");
            rightJoystick.x = Input.GetAxis("XboxRightJoystickHorizontal");
            rightJoystick.y = Input.GetAxis("XboxRightJoystickVertical");
            aimDir.x        = Input.GetAxis("LeftJoystickHorizontal");
            aimDir.y        = Input.GetAxis("LeftJoystickVertical");
            triggerState.x  = Input.GetAxis("XboxLeftTrigger");
            triggerState.y  = Input.GetAxis("XboxRightTrigger");
        }

        else
        {
            jump            = 0.0f;
            rightJoystick.x = 0;
            rightJoystick.y = 0;
            aimDir.x        = 0;
            aimDir.y        = 0;
            triggerState.x = -1;
            triggerState.y = -1;
        }

        if (speed > maxSpeed)
            speed = maxSpeed;


        /* The slide capacity disables all other capacity */
        if (!isSliding)
        {
            /* We record the time the player has pressed the jump button for 
             * different height the character can reach depending on this time 
             * (with a limit see in FixedUpdate below)*/
            if (jump >= 1f)
            {
                jumpDuration += Time.deltaTime;
                
            }
            else
            {
                isJumping       = false;
                jumpDuration    = 0f;
            }

            /* The character can jump if he start from the ground (not already jumping)
             * and the player asks for it */
            if (controller.isGrounded && isJumping == false)
            {
                if (jump > 0f)
                    isJumping = true;
            }

            /* if the player as pressed the jump button more than 
             * what he can do we force him to go down (only he's not climbing)*/
            if (jumpDuration > jumpDurationThreshold && !isClimbing)
                jump = 0f;

        }
       
        /* look if the player wants to aim on the enemy (Left JoyStick) and on what plane (in depth if L2 maintained)*/
        if (triggerState.x >= 0.0f && aimDir.sqrMagnitude > 0.0f && !isSliding)
            AimInDepth();
        else if (triggerState.x <= -1.0f && aimDir.sqrMagnitude > 0.0f)
        {
            Aim();
        }
        else
        {
            RaycastHit lookInFront;
            if (Physics.Raycast(transform.position, transform.forward, out lookInFront, range))
            {
                if (lookInFront.collider.CompareTag("Enemy"))
                {
                    targetEnemy     = lookInFront.collider.gameObject.GetComponent<Enemy>();
                    float aimInHead = 0;
                    
                    if (!targetEnemy.GetComponent<EnemyDroneMovement>())
                        aimInHead = transform.transform.localScale.y / 2.0f;

                    lockDir         = (targetEnemy.transform.position + targetEnemy.transform.up * aimInHead) - weapon.firePoint.position;
                    Debug           .DrawRay(weapon.firePoint.position, lockDir, Color.yellow);
                    weapon.firePoint.LookAt(weapon.firePoint.position + lockDir);
                    weapon.firePoint.Rotate(90, 0, 0);
                }
                else
                { 
                    targetEnemy     = null;
                    lockDir         = weapon.firePoint.forward;
                }
            }
            else
            {
                targetEnemy = null;
                lockDir     = weapon.firePoint.forward;
            }
        }

        if (targetEnemy)
            animator.SetBool("Aiming", true);
        else
            animator.SetBool("Aiming", false);

        /* Shoot if R2 maintained*/
        if (triggerState.y >= 0.0f)
        {
            int bulletNb = weapon.bulletNb;
            weapon.Shoot();

            if (bulletNb > weapon.bulletNb)
            {
                animator.SetBool("Shooting", true);
            }
            else
            {
                animator.SetBool("Shooting", false);
            }
        }

        /* count of time for scoring */
        timer       += Time.deltaTime;
        pointTimer  += Time.deltaTime;

        if (!WinScreen.gameIsWin && timer >= timeForLossOfPoint && pointTimer >= 1.0f)
        {
            timeScore -= pointLostPerSec;
            pointTimer = 0f;
        }
        else if (!WinScreen.gameIsWin && speed == maxSpeed && pointTimer >= 1.0f)
        {
            bonusScore += pointBySpeed;
            pointTimer = 0f;
        }

        GameMgr.Instance.SetSpeed(speed);
        GameMgr.Instance.SetDash(dashNb);
    }

    private Vector3 velocity      = Vector3.zero;

    private void FixedUpdate()
    {
        if (PauseMenu.GameIsPaused || WinScreen.gameIsWin || TrainingRead.Read)
            return;

        Vector3 newAccel = Vector3.zero;


        /* We update the old array of ennemy if an ennemy has been killed to avoid
         * trying to kill an already dead ennemy */
        if (updateEnemy)
        {
            depthEnemies = enemiesInDepth.GetComponentsInChildren<Enemy>();
            updateEnemy  = false;
        }

        SortEnemies();


        if (targetEnemy != null && targetEnemy.isAlive && targetImage)
        {
            targetImage.enabled = true;
            Vector3 targetPosition              =  playerCam.cam.WorldToViewportPoint(targetEnemy.transform.position);
            targetImage.transform.localPosition = new Vector3(targetPosition.x * uiRectTransform.sizeDelta.x, targetPosition.y * uiRectTransform.sizeDelta.y, 0f) - new Vector3(uiRectTransform.sizeDelta.x/2.0f, uiRectTransform.sizeDelta.y/2.0f,0f);
        }
        else if (targetImage)
        {
            targetImage.enabled = false;
        }

        /* We're looking to the information of movement given by the checkpoint we've encountered
         * to guide ourselves in the level*/
        if (currentZoneTransform != null)
        {
            /* Rotation... */
            transform           .Rotate(Vector3.up * currentZoneTransform.rotation.y * speed);
            Quaternion Rotation = Quaternion.Euler(0.0f, (transform.rotation.eulerAngles.y - 90),0.0f);



            /*...and Translation (the Character is always going right but we rotate him in wich
             * he can turn around and do all the level)*/
            newAccel            = Rotation * Vector3.right * speed * currentZoneTransform.speed;
    
            if (currentZoneTransform.rotation == Vector3.zero)
                velocity.z  = newAccel.z = 0.0f;

        }

        /* update Animation value */
        animator.SetFloat("Speed"       , speed);
        animator.SetBool ("isGrounded"  , controller.isGrounded);
        animator.SetBool ("isClimbing"  , isClimbing);
        animator.SetBool ("isJumping"   , isJumping);
        animator.SetBool ("isSliding"   , isSliding);

        if (!controller.isGrounded)
            animator.SetFloat("jumpDuration", animator.GetFloat("jumpDuration") + Time.fixedDeltaTime);
        else
            animator.SetFloat("jumpDuration", 0.0f);

        /* We look if the Player can climb and wants to climb... */
        if (jump >= 1f && isClimbing)
        {
            newAccel.y = climbSpeed;
        }
        /* ...or if the Player wants to jump and can jump/can jump higher 
         * as he can continue to gain height for a duration of jumpDurationThreshold */
        else if (isJumping && jumpDuration < jumpDurationThreshold)
        {
            newAccel.y = jumpSpeed;

        }
        else if (!controller.isGrounded)
        {
            /* Add gravity (considered an acceleration) */
            newAccel.y = Physics.gravity.y * gravityScale * Time.fixedDeltaTime;
        }

        if (newAccel.y >= 0.0f)
            velocity    = Vector3.Lerp(velocity, newAccel, upCoeff * Time.fixedDeltaTime);
        else
            velocity    = Vector3.Lerp(velocity, newAccel, downCoeff * Time.fixedDeltaTime);

        if (pushInfo.pushedCoeff >= 0.0f)
            velocity    = Vector3.Lerp(velocity, pushInfo.pushedDirection, pushInfo.pushedCoeff * Time.fixedDeltaTime);

        /* Look if the player wants to dash or Slide */
        if ((rightJoystick.sqrMagnitude >= 0.8))
        {
            /* look if he wants to Slide (Joystick headed down) and can (not already sliding) */
            if (rightJoystick.y <= -0.5 && !isSliding)
            {
                /* Setting the new collider of slide */

                ChangeControllerInfo(SlideControllerInfo);
                isSliding = true;
                slideDur = slideDuration;
                controller.center = Vector3.up * 0.25f;
            }
            /* look if he wants to dash and can */
            else if (!isSliding && cooldown <= 0.0f && dashNb > 0)
            {
                dashNb--;
                cooldown = dashCooldown;
                dashDur = dashDuration;
                dashDir = playerCam.cam.transform.TransformDirection(rightJoystick.normalized);

                if (dashParticles)
                {
                    dashParticles.Play();
                    dashParticles.transform.LookAt(dashParticles.transform.position + new Vector3(dashDir.x, dashDir.y));
                }
                if (dashAudio)
                {
                    Instantiate(dashAudio, transform);
                }
            }
        }

        /* Cooldowns of slides and Dashs */
        if (slideDur <= 0.0f && isSliding)/* end of slide */
        {
            RaycastHit raycast;

            if (Physics.Raycast(transform.position + Vector3.up * SlideControllerInfo.y, Vector3.up, out raycast, controllerInfo.y - 0.6f))
            {
                if (raycast.collider.CompareTag("Zone"))
                    loadSave();
            }


            /* Change collider to the real one */
            ChangeControllerInfo(controllerInfo);
            controller.center   = Vector3.up * 0.9f;
            isSliding           = false;
        }
        else if (dashDur >= 0.0f)/* Dash */
        {
            /* Dash for the duration in the direction dashDir with the velocity dashVelocity*/
            velocity = dashDir * dashVelocity;
            dashDur -= Time.fixedDeltaTime;
        }

        else if (dashDur < 0.0f && dashParticles.isPlaying)
            dashParticles.Stop();

        /* Decrementing the cooldowns */
        else if (slideDur >= 0.0f)
        {
            slideDur -= Time.fixedDeltaTime;
        }
        else if (cooldown >= 0.0f)
        {
            cooldown -= Time.fixedDeltaTime;
        }

        /* Dash reload cooldown and reload dash*/
        if (dashNb < dashMaxNb)
        {
            reloadCooldown -= Time.fixedDeltaTime;
            if (reloadCooldown <= 0.0f)
            {
                dashNb++;
                reloadCooldown = dashReloadCooldown;
                
                if (dashReloadAudio)
                {
                    Instantiate(dashReloadAudio,transform);    
                }
            }
        }

        if (currentZoneTransform && currentZoneTransform.rotation == Vector3.zero && pushInfo.pushedCoeff <= 0f)
        {
            if (transform.position.z != zLock)
            {
                velocity.z = (zLock - transform.position.z) * speed;
            }
        }

        /* Move the Character with the velocity vector 
         * (conidered as a speed because multiplied with time)
         * (the gravity would be then considered as acceleration because multiplied with time squared)*/
        controller.Move(velocity * Time.fixedDeltaTime);
    }
}