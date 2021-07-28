using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField, Range(0f, 50f)] private float speed = 10f;
    [SerializeField, Range(0f, 10f)] private float lifeTime = 2f;
    [SerializeField, Range(0f, 10f)] private float despawnEpsilon = 5f;
    private Rigidbody rb = null;
    // Start is called before the first frame update


    private float timer = 0.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;

        RaycastHit lookForCollision;
        if (Physics.Raycast(transform.position, transform.forward, out lookForCollision))
        {
            if (!lookForCollision.collider.CompareTag("Enemy") && !lookForCollision.collider.CompareTag("Enemy Bullet"))
            {
                float timeBeforeCollide = lookForCollision.distance / speed;

                if (timeBeforeCollide <= lifeTime)
                {
                    lifeTime = timeBeforeCollide + despawnEpsilon * Time.deltaTime;
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.GameIsPaused || WinScreen.gameIsWin)
        {
            rb.velocity = Vector3.zero;
            return;
        }
        else
        {
            rb.velocity = transform.forward * speed;
            timer       += Time.deltaTime;
        }
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }

    }

    void OnTriggerEnter(Collider collider)
    {
        Destroy(gameObject);

        if (collider.gameObject.name == "Player")
        {
            Player player = collider.gameObject.GetComponent<Player>();
            player.speed -= player.speedSlowShot;
            if (player.speed < 0)
                player.speed = 0;
            player.UpdatePreshotPoint();
        }
    }
}
