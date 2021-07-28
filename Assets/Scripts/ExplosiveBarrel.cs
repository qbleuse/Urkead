using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    [SerializeField, Range(0.0f, 20.0f)] private float range = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player Bullet"))
        {
            Enemy[] enemies = FindObjectsOfType<Enemy>();

            foreach (Enemy enemy in enemies)
            {
                if ((enemy.transform.position - transform.position).magnitude < range)
                {
                    enemy.alive = false;
                    enemy.gameObject.SetActive(false);
                }
            }
        }
    }
}
