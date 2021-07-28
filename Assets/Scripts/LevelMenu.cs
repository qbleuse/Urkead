using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    [SerializeField] private List<GameObject> buttons;
    private int buttonsIndex = 0;
    [SerializeField] private GameObject uiButton;
    [SerializeField] private GameObject fleche;
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
        float horizontal = Input.GetAxis("LeftJoystickHorizontal");

        if (smooth > 0f)
            smooth -= Time.deltaTime;
        else if (vertical <= -0.75f && buttonsIndex < 6)
        {
            if (buttonsIndex == buttons.Count - 3)
            {
                buttonsIndex += 2;
                borderSound?.PlayOneShot(borderSound.clip);
            }
            else if (buttonsIndex == buttons.Count - 2)
            {
                buttonsIndex++;
                borderSound?.PlayOneShot(borderSound.clip);
            }
            else
            {
                buttonsIndex += 3;
                borderSound?.PlayOneShot(borderSound.clip);
            }
            
            smooth = 0.25f;
        }
        else if (vertical >= 0.75f && buttonsIndex > 2)
        {
            buttonsIndex -= 3;
            borderSound?.PlayOneShot(borderSound.clip);
            smooth = 0.25f;
        }
        else if (horizontal >= 0.75f && buttonsIndex != 2 && buttonsIndex < 5)
        {
            buttonsIndex ++;
            borderSound?.PlayOneShot(borderSound.clip);
            smooth = 0.25f;
        }
        else if (horizontal <= -0.75f && buttonsIndex != 0 && buttonsIndex != 3 && buttonsIndex != 6)
        {
            buttonsIndex --;
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
        if (buttonsIndex == 6)
        {
            uiButton.SetActive(false);
            fleche.SetActive(true);
            fleche.transform.position = buttons[buttonsIndex].transform.position;
        }   
        else
        {
            uiButton.SetActive(true);
            fleche.SetActive(false);
            uiButton.transform.position = buttons[buttonsIndex].transform.position + new Vector3(0f, 2.2f, 0f);
        }
            
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(buttonsIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
