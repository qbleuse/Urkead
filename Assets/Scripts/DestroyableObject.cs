using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour
{
    [SerializeField]                     private GameObject toCreate  = null;
    [SerializeField, Range(0.0f, 10.0f)] private float      magnitude = 0.0f;
    [SerializeField, Range(0.0f, 10.0f)] private float      duration  = 0.0f;
    private bool alive = true;

    public bool isAlive { get => alive; set { gameObject.SetActive(true); alive = true; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collider)
    {
        if (toCreate)
            Instantiate(toCreate, transform.position, toCreate.transform.rotation, transform.parent);

        if (magnitude > 0.0f)
            Player.Instance.playerCam.MakeShake(duration,magnitude);

        alive = false;
        gameObject.SetActive(false);
    }
}
