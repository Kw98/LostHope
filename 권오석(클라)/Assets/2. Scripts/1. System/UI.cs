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

    protected override void Awake()
    {
        base.Awake();
        if (GameManager.Instance == null)
        {
            SceneManager.LoadScene("Title");
        }
    }

    public void OnExit()
    {
        SceneManager.LoadScene("Title");
    }

    public void OnReStart()
    {
        SceneManager.LoadScene("Town");
    }

    void Update()
    {
        if (Define.state != GameState.Play)
            return;

        UpdateUI();
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
}
