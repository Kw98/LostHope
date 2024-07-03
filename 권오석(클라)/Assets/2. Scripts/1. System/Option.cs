using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Option : MonoBehaviour
{
    [Header("Option")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject state;
    public static bool GameIsPaused = false;

    // Update is called once per frame
    void Update()
    {
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

    public void Pause()
    {
        pauseMenu.SetActive(true);
        state.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        state.SetActive(false);
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
}
