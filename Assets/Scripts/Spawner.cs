using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject obj;
    [SerializeField] float waitingTime = 10f;
    private float wait;

    void Start()
    {
        wait = waitingTime;
        GameObject car = Instantiate(obj, gameObject.transform.position, gameObject.transform.rotation, transform.parent);
        //car.transform.Rotate(new Vector3(0, 90, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if (waitingTime > 0)
        {
            waitingTime -= Time.deltaTime;
        }
        else if (waitingTime <= 0)
        {
            waitingTime = wait;
            GameObject car = Instantiate(obj, gameObject.transform.position, gameObject.transform.rotation,transform.parent);
            //car.transform.Rotate(new Vector3(0, 90, 0));
        }
    }
}
