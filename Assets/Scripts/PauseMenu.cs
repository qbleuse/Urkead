using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
                        public static bool              GameIsPaused    = false;
    [SerializeField]    public         GameObject       pauseMenuUI     = null;
    [SerializeField]    private        List<GameObject> buttons         = null;
                        private        int              buttonsIndex    = 0;
    [SerializeField]    private        GameObject       uiButton        = null;
                        private        AudioSource      borderSound     = null;
                        private        float            smooth          = 0.25f;

    void Start()
    {
        SelectButton();
        borderSound = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (GameMgr.controllerType)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button9))
            {
                if (GameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button7))
            {
                if (GameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        if (GameIsPaused)
        {
            float horizontal = Input.GetAxis("LeftJoystickHorizontal");

            if (smooth > 0f)
                smooth -= Time.deltaTime;
             else if (horizontal <= -0.5f)
             {
                buttonsIndex--;
                borderSound?.PlayOneShot(borderSound.clip);
                smooth = 0.25f;
             }
             else if (horizontal >= 0.5f)
             {
                buttonsIndex++;
                borderSound?.PlayOneShot(borderSound.clip);
                smooth = 0.25f;
             }

             if (buttonsIndex == buttons.Count)
                buttonsIndex = buttons.Count - 1;
             else if (buttonsIndex < 0)
                buttonsIndex = 0;
            SelectButton();

            if (GameMgr.controllerType)
             {
                if (Input.GetButtonDown("Jump"))
                {
                    if (buttons[buttonsIndex].GetComponent<Button>() != null)
                        buttons[buttonsIndex].GetComponent<Button>().onClick.Invoke();
                }
             }
             else
             {
                if (Input.GetButtonDown("XboxJump"))
                {
                    if (buttons[buttonsIndex].GetComponent<Button>() != null)
                        buttons[buttonsIndex].GetComponent<Button>().onClick.Invoke();
                }
             }
        }
    }

    public void SelectButton()
    {
        uiButton.GetComponent<RectTransform>().sizeDelta = buttons[buttonsIndex].GetComponent<RectTransform>().sizeDelta + new Vector2(5f, 5f);
        uiButton.transform.position = buttons[buttonsIndex].transform.position;
    }

    public void Resume()
    {
        pauseMenuUI     .SetActive(false);
        GameIsPaused    = false;
    }

    void Pause()
    {
        pauseMenuUI     .SetActive(true);
        GameIsPaused    = true;
    }

    public void LoadMenu()
    {
        pauseMenuUI     .SetActive(false);
        SceneManager    .LoadScene(0);
        GameIsPaused    = false;
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameIsPaused = false;
    }
}
