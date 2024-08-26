using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : Singleton<UI>
{
    [SerializeField] private Player p;

    [Header("HP")]
    [SerializeField] private Image curHPImage;
    [SerializeField] private TextMeshProUGUI hpTxt;

    [Header("Level")]
    [SerializeField] private TextMeshProUGUI lvTxt;
    [SerializeField] private Image curExpImage;

    [Header("Status")]
    [SerializeField] private TextMeshProUGUI statPointTxt;
    [SerializeField] private TextMeshProUGUI hpPointTxt;
    [SerializeField] private TextMeshProUGUI meleeTxt;
    [SerializeField] private TextMeshProUGUI rangeTxt;

    [Header("Panel")]
    [SerializeField] private GameObject deadPanel;
    [SerializeField] private GameObject clearPanel;

    [Header("Weapon")]
    [SerializeField] private Weapon[] weapon;
    [SerializeField] private TextMeshProUGUI ammoTxt;
    public GameObject[] curWeapon;

    // Update is called once per frame
    void Update()
    {
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
            string levelString = p.level.ToString().PadLeft(2, '0');
            lvTxt.text = "LV " + levelString;

            //Exp
            float expFillAmount = (float)p.curExp / p.maxExp;
            curExpImage.fillAmount = expFillAmount;

            //Ammo
            ammoTxt.text = weapon[1].currentAmmo + " / " + weapon[1].maxAmmo 
                            + " | " + weapon[1].reserveAmmo;

            //Status
            statPointTxt.text = "" + p.statPoint;
            hpPointTxt.text = "" + p.maxHP;
            meleeTxt.text = "" + weapon[0].meleeDamage;
            rangeTxt.text = "" + weapon[1].rangeDamage;
        }
    }

    public void DeadPanel()
    {
        deadPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ClearPanel()
    {
        clearPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void UpgradeHp()
    {
        if (p.statPoint > 0)
        {
            p.maxHP += 5;
            p.statPoint -= 1;
        }
    }

    public void UpgradeMeleeDamage()
    {
        if (p.statPoint > 0)
        {
            weapon[0].IncreaseMeleeDamage(1);
            p.statPoint -= 1;
        }
    }

    public void UpgradeRangeDamage()
    {
        if (p.statPoint > 0)
        {
            weapon[1].IncreaseRangeDamage(1);
            p.statPoint -= 1;
        }
    }
}
