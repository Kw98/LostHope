using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreUI : Singleton<StoreUI>
{
    public static bool GameIsPaused = false;

    [SerializeField] private GameObject storeUI;
    [SerializeField] private GameObject[] itemObj;

    public void OpenStore()
    {
        storeUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void CloseStore()
    {
        storeUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Buy(int index)
    {
        itemObj[index].SetActive(true);
    }
}
