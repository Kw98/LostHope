using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject optionButton;
    [SerializeField] private GameObject exitGame;

    [Header("Option")]
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject blurPanel;
    [SerializeField] private GameObject optionExit;

    void Start()
    {
        startButton.SetActive(true);
        optionButton.SetActive(true);
        exitGame.SetActive(true);
        optionsMenu.SetActive(false);
        blurPanel.SetActive(false);
    }

    public void StartGame()
    {
        Loading.LoadScene("Town");
    }

    public void ShowOptions()
    {
        optionsMenu.SetActive(true);
        blurPanel.SetActive(true);
    }

    public void HideOptions()
    {
        optionsMenu.SetActive(false);
        blurPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
