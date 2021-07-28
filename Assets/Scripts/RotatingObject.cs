using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    float rotateDuration = 3;
    bool goRight = false;
    [SerializeField] float rotateSpeed = 8; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.GameIsPaused || WinScreen.gameIsWin)
            return;

        if (rotateDuration > 0)
         {
             rotateDuration -= Time.deltaTime;
         }
         else if (rotateDuration <= 0)
         {
             goRight = !goRight;
             rotateDuration = 3;
         }

         if (goRight)
            transform.Rotate(new Vector3(1, 0, 0), (rotateSpeed * rotateDuration) * Time.deltaTime);
         else
            transform.Rotate(new Vector3(1, 0, 0), -(rotateSpeed * rotateDuration) * Time.deltaTime);
    }
}