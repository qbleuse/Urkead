using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public struct Save
{
    public Enemy[]              enemies;
    public EnemyDroneMovement[] drones;
    public Collectibles[]       collectibles;
    public Checkpoint           checkpoint;
    public MovingObject[]       movingObjects;
    public DestroyableObject[]  destroyableObjects;
}

public class GameMgr : MonoBehaviour
{
                     private static GameMgr         instance        = null;
    [SerializeField] private        Weapon          playerWeapon    = null;
    [SerializeField] private        Slider          slider          = null;
    [SerializeField] private        Image           fill            = null;
                     public         Gradient        gradient        = null;
    [HideInInspector]public  static bool            controllerType  = false;
    [SerializeField] private        Sprite          offDash         = null;
    [SerializeField] private        Sprite          onDash          = null;
    [SerializeField] private        Text            text            = null;
    [SerializeField] public         GameObject      UIScreen        = null;
    [SerializeField] public         Image           infiniteImage   = null;
    [SerializeField] private        ParticleSystem  uiParticle      = null;
    [SerializeField] private        float           startLifetimeCoeff = 1.0f;
    [SerializeField] private        float           startSpeedCoeff    = 1.0f;
                     private static Save            save;
    [SerializeField] private        Color           startColor;
    [SerializeField] private        Color           endColor;

    private Text ammoText = null;

    public static void LoadSave()
    {
        if (save.enemies != null)
        {
            foreach (Enemy enemy in save.enemies)
            {
                if (enemy && !enemy.isAlive)
                {
                    enemy.isAlive = true;
                }
            }
        }

        if (save.drones != null)
        {
            foreach (EnemyDroneMovement drone in save.drones)
            {
                if (drone)
                {
                    drone.isUp               = false;
                    drone.isMoving           = false;
                    drone.transform.position = drone.initPos;
                    drone.direction          = Vector3.right;
                    drone.currentPos         = Vector3.zero;
                    drone.gameObject.SetActive(true);
                    drone.transform.SetParent(drone.initCheckpoint);
                }
            }
        }

        if (save.collectibles != null)
        {
            foreach (Collectibles collectibles in save.collectibles)
            {
                if (collectibles && !collectibles.isAlive)
                {
                    collectibles.isAlive = true;
                }
            }
        }

        if (save.movingObjects != null)
        {
            foreach (MovingObject movingObject in save.movingObjects)
            {
                if (movingObject)
                    movingObject.Reset();
            }
        }

        if (save.destroyableObjects != null)
        {
            foreach (DestroyableObject destroyableObject in save.destroyableObjects)
            {
                if (destroyableObject && !destroyableObject.isAlive)
                    destroyableObject.isAlive = true;
            }
        }

        Player.Instance.transform.position              = Player.Instance.PlayerSave.position;
        Player.Instance.speed                           = Player.Instance.PlayerSave.speed;
        Player.Instance.weapon.bulletNb                 = Player.Instance.PlayerSave.bullet;
        Player.Instance.killScore                       = Player.Instance.PlayerSave.killScore;

        GameMgr.Instance                                .BulletUpdate(Player.Instance.weapon.bulletNb);

        Player.Instance.playerCam.transform.position    = Player.Instance.transform.position + save.checkpoint.camPosition;
        Player.Instance.transform                       .Rotate(-Player.Instance.transform.rotation.eulerAngles);
        Player.Instance.transform                       .Rotate(save.checkpoint.initRotation);
        Player.Instance.playerCam                       .CamChangeInfo(save.checkpoint);
        Player.Instance.updateEnemy                     = true;
        
    }

    public void UpdateSave(Checkpoint newZone)
    {
        if (newZone.save || !save.checkpoint)
        {
            save.checkpoint = newZone;
        }
        else
        {
            return;
        }


        if (save.enemies != null)
        {
            foreach (Enemy enemy in save.enemies)
            {
                if (enemy && !enemy.isAlive)
                {
                    Destroy(enemy.gameObject);
                }
            }
        }

        if (save.collectibles != null)
        {
            foreach (Collectibles collectibles in save.collectibles)
            {
                if (collectibles && !collectibles.isAlive)
                {
                    Destroy(collectibles.gameObject);
                }
            }
        }

        if (save.movingObjects != null)
        {
            foreach (MovingObject movingObject in save.movingObjects)
            {
                if (movingObject)
                {
                    movingObject.loadPos = movingObject.transform.position;
                    movingObject.saveDirection = movingObject.direction;
                }
            }
        }
        
        if (save.destroyableObjects != null)
        {
            foreach (DestroyableObject destroyableObject in save.destroyableObjects)
            {
                if (destroyableObject && !destroyableObject.isAlive)
                    Destroy(destroyableObject.gameObject);
            }
        }
    }

    /* Controller */
    private bool ControllerChoose()
    {
        string[] names = Input.GetJoystickNames();
        for (int x = 0; x < names.Length; x++)
        {
            if (names[x].Length == 19)
            {
                return true;
            }
            if (names[x].Length == 33)
            {
                return false;
            }
        }

        return false;
    }

    public static GameMgr Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindGameObjectWithTag("GameMgr")?.GetComponent<GameMgr>();
            return instance;
        }
    }

    //////////////////////////////////////////////////////////////////////////// BULLET
    public void BulletUpdate(int value)
    {
        if (value == int.MaxValue)
        {
            ammoText.enabled = false;
            if (infiniteImage)
                infiniteImage.enabled = true;
        }
        else
        {
            ammoText.enabled = true;
            if (infiniteImage)
                infiniteImage.enabled = false;
            ammoText.text = value.ToString();
        }
    }

    ////////////////////////////////////////////////////////////////////////// SPEED
    public void SetMaxSpeed(float speed)
    {
        slider.maxValue = speed;
    }

    public void SetMinSpeed(float minSpeed)
    {
        slider.minValue = minSpeed;
    }

    public void SetSpeed(float speed)
    {
        slider.value = speed;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    ///////////////////////////////////////////////////////////////////// DASH

    private int         dashNb      = 0;
    private Image       dash1       = null;
    private Image       dash2       = null;
    private Animation   dash1Anim   = null;
    private Animation   dash2Anim   = null;

    public void SetDash(int dash)
    {
        if (dash == 2)
        {
            dash1.sprite = onDash;
            dash2.sprite = onDash;
        }
        else if (dash == 1)
        {
            dash1.sprite = offDash;
            dash2.sprite = onDash;
        }
        else if (dash == 0)
        {
            dash1.sprite = offDash;
            dash2.sprite = offDash;
        }

        if (dash == 2 && dashNb == 1)
        {
            dash1Anim.Play();
        }
        else if (dash == 1 && dashNb == 0)
        {
            dash2Anim.Play();
        }
        else
        {
            if (!dash1Anim.isPlaying)
                dash1Anim.Stop();
            if (!dash2Anim.isPlaying)
                dash2Anim.Stop();
        }

        dashNb = dash;
    }

    ///////////////////////////////////////////////////////////////// TIME
    public void Timer(Text text)
    {
        float timeInMin = Player.Instance.timer;
        timeInMin       = Mathf.Floor(timeInMin/60.0f);
        float timeInSec = Mathf.Floor(Player.Instance.timer - (timeInMin * 60.0f));
        text.text       = timeInMin.ToString() + " : " + timeInSec.ToString() + " : " + Mathf.Floor((Player.Instance.timer - (timeInMin * 60.0f) - timeInSec) * 100f).ToString();
    }
   

private void Awake()
    {
        controllerType = ControllerChoose();
    }

    // Start is called before the first frame update
    void Start()
    {
        ammoText    = GameObject.FindGameObjectWithTag("TextUI")?.GetComponent<Text>();
        dash1       = GameObject.FindGameObjectWithTag("Dash1")?.GetComponent<Image>();
        dash2       = GameObject.FindGameObjectWithTag("Dash2")?.GetComponent<Image>();
        dash1Anim   = dash1?.GetComponent<Animation>();
        dash2Anim   = dash2?.GetComponent<Animation>();

        if (playerWeapon != null)
        {
            playerWeapon.OnShootEvent += BulletUpdate;
            if (playerWeapon.LimitedBullet)
                BulletUpdate(playerWeapon.bulletNb);
            else
                BulletUpdate(int.MaxValue);
        }

        save.checkpoint         = null;
        save.enemies            = FindObjectsOfType<Enemy>();
        save.collectibles       = FindObjectsOfType<Collectibles>();
        save.movingObjects      = FindObjectsOfType<MovingObject>();
        save.drones             = FindObjectsOfType<EnemyDroneMovement>();
        save.destroyableObjects = FindObjectsOfType<DestroyableObject>();
    }

    private void Update()
    {
        if (Player.Instance)
            Timer(text);


        if (uiParticle)
        {
            ParticleSystem.MainModule main = uiParticle.main;
            main.startLifetimeMultiplier = Player.Instance.speed * startLifetimeCoeff;
            main.startSpeedMultiplier = Player.Instance.speed * startSpeedCoeff;
            main.startColor = Color.Lerp(startColor,
                                         endColor,
                                         (Player.Instance.speed - Player.Instance.MinSpeed) / (Player.Instance.MaxSpeed - Player.Instance.MinSpeed));
        }
    }

}
