using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UI : Singleton<UI>
{
    [SerializeField] private Player p;

    [Header("HP")]
    [SerializeField] private Image curHPImage;
    [SerializeField] private TextMeshProUGUI hpTxt;

    [Header("Level")]
    [SerializeField] private TextMeshProUGUI lvTxt;

    [Header("Panel")]
    [SerializeField] private GameObject deadPanel;
    [SerializeField] private GameObject clearPanel;

    [Header("Option")]
    [SerializeField] private GameObject pauseMenuCanvas;
    public static bool GameIsPaused = false;

    void Update()
    {
        UpdateUI();

        if (Input.GetKeyDown(KeyCode.Escape))
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

    public void UpdateUI()
    {
        if (p != null)
        {
            //HP
            float hpFillAmount = (float)p.curHP / p.maxHP;
            curHPImage.fillAmount = hpFillAmount;
            hpTxt.text = string.Format("{0}/{1}", p.curHP, p.maxHP);

            //LV
            string levelString = p.curLevel.ToString().PadLeft(2, '0');
            lvTxt.text = "LV " + levelString;
        }
    }

    public void Pause()
    {
        pauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Resume()
    {
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void ToMain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }

    public void ToTown()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Town");
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


    public void DeadPanel()
    {
        deadPanel.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void ClearPanel()
    {
        clearPanel.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
}
