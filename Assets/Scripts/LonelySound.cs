using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform)),RequireComponent(typeof(AudioSource))]
public class LonelySound : MonoBehaviour
{ 
    private AudioSource sound = null;
    private float       timer = 0f;


    private void OnDestroy()
    {
        sound.Stop();
    }

    // Start is called before the first frame update
    void Start()
    {
        sound   = GetComponent<AudioSource>();
        sound   .Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!sound.loop)
        {
            if (PauseMenu.GameIsPaused || WinScreen.gameIsWin)
            {
                sound.Pause();
                return;
            }
            else
            {
                sound.UnPause();
            }
            timer += Time.deltaTime;
            if (timer >= sound.clip.length)
            {
                Destroy(gameObject);
            }
            
        }
    }
}
