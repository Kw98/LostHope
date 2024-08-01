using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class MainMenu : MonoBehaviour
{
    [Title("Main Menu")]  // 제목을 추가하여 섹션을 명확히 구분합니다.
    [SerializeField, BoxGroup("Main")] private GameObject startButton;
    [SerializeField, BoxGroup("Main")] private GameObject optionButton;
    [SerializeField, BoxGroup("Main")] private GameObject exitGame;

    [Title("Options Menu")]
    [SerializeField, BoxGroup("Option")] private GameObject optionsMenu;
    [SerializeField, BoxGroup("Option")] private GameObject blurPanel;
    [SerializeField, BoxGroup("Option")] private GameObject optionExit;


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
