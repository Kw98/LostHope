using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public static event Action<float> OnSceneLoadProgress;
    public static string nextScene;
    [SerializeField] Image loadingBar;

    private static float loadingTime;
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        loadingTime = 3f;
        SceneManager.LoadScene("Loading");
    }

    //public static void UILoad(string sceneName)
    //{
    //    SceneManager.LoadScene(sceneName);
    //    SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
    //}


    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        float totalTime = 0.0f;

        while (!op.isDone && totalTime < loadingTime)
        {
            yield return null;
            totalTime += Time.deltaTime;

            float fillAmount = totalTime / loadingTime;
            if (op.progress < 0.9f)
            {
                loadingBar.fillAmount = fillAmount;
                if (loadingBar.fillAmount >= fillAmount)
                {
                    totalTime = 0f;
                }
            }
            else
            {
                loadingBar.fillAmount = fillAmount;
                if (loadingBar.fillAmount == 1.0f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }

            OnSceneLoadProgress?.Invoke(op.progress);
        }
    }
}
