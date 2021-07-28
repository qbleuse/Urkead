using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectibles : MonoBehaviour
{
    private enum CollectiblesBonus
    {
        BULLET,
        SPEED
    }

    [SerializeField] private CollectiblesBonus Bonus;
    [SerializeField] private int            bullet          = 10;
    [SerializeField] private float          speed           = 0.1f;
    [SerializeField] private float          rotatingSpeed   = 1.0f;
    [SerializeField] private GameObject     sound           = null;
                     private bool           alive           = true;


    public bool isAlive { get => alive; set {alive = true; gameObject.SetActive(true);}}

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotatingSpeed * Time.deltaTime * Vector3.up);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            if (Bonus == CollectiblesBonus.BULLET)
            {
                
                if (Player.Instance.weapon.LimitedBullet)
                {
                    Player.Instance.weapon.bulletNb += bullet;
                    GameMgr.Instance.BulletUpdate(Player.Instance.weapon.bulletNb);
                }
                else
                {
                    GameMgr.Instance.BulletUpdate(int.MaxValue);
                }
            }

            if (Bonus == CollectiblesBonus.SPEED)
            {
                Player.Instance.speed += speed;
            }

            if (sound)
            {
                Instantiate(sound, transform.position, Quaternion.identity);
            }

            alive = false;
            gameObject.SetActive(false);
        }
    }
}
