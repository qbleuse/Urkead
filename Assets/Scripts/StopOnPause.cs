using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopOnPause : MonoBehaviour
{

    private Animator animator = null;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (animator && PauseMenu.GameIsPaused || WinScreen.gameIsWin || TrainingRead.Read)
        {
            animator.enabled = false;
            return;
        }
        else if (animator)
            animator.enabled = true;

    }
}
