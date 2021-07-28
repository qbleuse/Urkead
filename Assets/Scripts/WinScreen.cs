using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
                     public     static  bool             gameIsWin      = false;
    [SerializeField] private            List<GameObject> buttons        = null;
    [SerializeField] public             GameObject       pauseMenuUI    = null;
                     private            int              buttonsIndex   = 0;
    [SerializeField] private            GameObject       uiButton       = null;
                     private            float            smooth         = 0.25f;
    [SerializeField] private            List<Text>       texts          = null;
    [SerializeField] private            float            animDuration   = 5.0f;
                     private            Coroutine        animCoroutine  = null;
                     private            AudioSource      borderSound    = null;
                     private            AudioSource      animationSound = null;
    void Start()
    {
        SelectButton();
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources[0].clip.name.Contains("Border"))
        {
            borderSound     = audioSources[0];
            if (audioSources.Length > 1)
                animationSound  = audioSources[1];
        }

        else
        {
            animationSound  = audioSources[0];
            if (audioSources.Length > 1)
                borderSound     = audioSources[1];
        }
    }

    void Update()
    {
        if (gameIsWin)
        {
            if (GameMgr.Instance.UIScreen)
                GameMgr.Instance.UIScreen.SetActive(false);

            pauseMenuUI.SetActive(true);
            float vertical = Input.GetAxis("LeftJoystickHorizontal");

            foreach (Text text in texts)
            {
                if (text.name.Contains("FinalScore"))
                {
                    if (animCoroutine == null && text.text.Length == 0)
                    {
                        animCoroutine = StartCoroutine(ScoreAnimation(Player.Instance.finalScore, text));
                    }
                }
                else if (text.name.Contains("Timer"))
                {
                    float timeInMin = Player.Instance.timer;
                    timeInMin = Mathf.Floor(timeInMin / 60.0f);
                    float timeInSec = Mathf.Floor(Player.Instance.timer - (timeInMin * 60.0f));
                    text.text = timeInMin.ToString() + " : " + timeInSec.ToString() + " : " + Mathf.Floor((Player.Instance.timer - (timeInMin * 60.0f) - timeInSec) * 100f).ToString();
                }
                else if (text.name.Contains("Kill"))
                {
                    text.text = Player.Instance.killScore.ToString();
                }
                else if (text.name.Contains("Bonus"))
                {
                    text.text = Player.Instance.bonusScore.ToString();
                }
                else if (text.name.Contains("Time"))
                {
                    text.text = Player.Instance.timeScore.ToString();
                }
                else if (text.name.Contains("Best"))
                {
                    float timeInMin = TimeSave.bestTime;
                    timeInMin = Mathf.Floor(timeInMin / 60.0f);
                    float timeInSec = Mathf.Floor(TimeSave.bestTime - (timeInMin * 60.0f));
                    text.text = timeInMin.ToString() + " : " + timeInSec.ToString() + " : " + Mathf.Floor((TimeSave.bestTime - (timeInMin * 60.0f) - timeInSec) * 100f).ToString();
                }
            }

            if (animCoroutine != null)
                vertical = 0;

            if (smooth > 0f)
                smooth -= Time.deltaTime;
            else if (vertical >= 0.5f)
            {
                buttonsIndex++;
                borderSound?.PlayOneShot(borderSound.clip);
                smooth = 0.25f;
            }
            else if (vertical <= -0.5f)
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
    }

    public void SelectButton()
    {
        uiButton.GetComponent<RectTransform>().sizeDelta = buttons[buttonsIndex].GetComponent<RectTransform>().sizeDelta + new Vector2(5f, 5f);
        uiButton.transform.position = buttons[buttonsIndex].transform.position;
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameIsWin = false;
        pauseMenuUI.SetActive(false);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
        gameIsWin = false;
        pauseMenuUI.SetActive(false);
    }

    public void Next()
    {
        if (SceneManager.GetActiveScene().buildIndex == 6)
            SceneManager.LoadScene(8);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        gameIsWin = false;
        pauseMenuUI.SetActive(false);
    }

    private IEnumerator ScoreAnimation(int score, Text text)
    {
        int toScore = 0;

        int scoreToAdd = (int)Mathf.Floor(((float)score / animDuration) * Time.deltaTime);

        while (toScore < score)
        {
            toScore += scoreToAdd;
            text.text = toScore.ToString();

            if (!animationSound.isPlaying)
                animationSound.Play(); 

            yield return null;
        }

        animationSound.Stop();

        StopCoroutine(animCoroutine);
        animCoroutine = null; 
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
