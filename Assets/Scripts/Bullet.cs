using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField, Range(0f, 50f)]  private float       speed           = 20f;
    [SerializeField, Range(0f, 10f)]  private float       lifeTime        = 2f;
    [SerializeField, Range(0f, 100f)]  private float       despawnEpsilon   = 5f;
    private Rigidbody rb;

    private float timer = 0.0f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.up * speed;
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
            rb.velocity = transform.up * speed;
            timer += Time.deltaTime;
        }
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
