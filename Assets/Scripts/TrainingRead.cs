using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrainingRead : MonoBehaviour
{
    public static bool Read = false;
    [SerializeField] public GameObject trainingMenu = null;
    [SerializeField] private List<GameObject> buttons = null;
    private int buttonsIndex = 0;
    [SerializeField] private GameObject uiButton = null;

    void Start()
    {
        Read = true;
        SelectButton();
    }

    void Update()
    {
        if (Read)
        {
            Pause();

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
        else 
        {
            Resume();
        }
    }

    public void SelectButton()
    {
        uiButton.GetComponent<RectTransform>().sizeDelta = buttons[buttonsIndex].GetComponent<RectTransform>().sizeDelta + new Vector2(10f, 10f);
        uiButton.transform.position = buttons[buttonsIndex].transform.position;
    }

    public void Resume()
    {
        trainingMenu.SetActive(false);
        Read = false;
    }

    void Pause()
    {
        trainingMenu.SetActive(true);
        Read = true;
    }
}
