using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCar : MonoBehaviour
{
    [SerializeField, Range(0f, 50f)] private float speed = 20f;
    [SerializeField, Range(0f, 10f)] private float lifeTime = 10f;

    private float       timer = 0.0f;
    private Rigidbody   rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (PauseMenu.GameIsPaused || WinScreen.gameIsWin)
        {
            rb.velocity = Vector3.zero;
            return;
        }
        else
        {
            rb.velocity = transform.right * speed;
            timer += Time.deltaTime;
        }
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }        
    }
}
