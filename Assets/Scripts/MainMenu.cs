using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private List<GameObject> buttons;
    private int buttonsIndex = 0;
    [SerializeField] private GameObject uiButton;
    private AudioSource borderSound = null;
    private float smooth = 0.25f;



    void Start()
    {
        SelectButton();
        borderSound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        float vertical = Input.GetAxis("LeftJoystickVertical");

        if (smooth > 0f)
            smooth -= Time.deltaTime;
        else if (vertical <= -0.75f)
        {
            buttonsIndex++;
            borderSound?.PlayOneShot(borderSound.clip);
            smooth = 0.25f;
        }

        else if (vertical >= 0.75f)
        {
            buttonsIndex--;
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

    public void SelectButton()
    {
        uiButton.GetComponent<RectTransform>().sizeDelta = buttons[buttonsIndex].GetComponent<RectTransform>().sizeDelta;
        uiButton.transform.position = buttons[buttonsIndex].transform.position;
    }

    public void Training()
    {
        SceneManager.LoadScene(7);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
