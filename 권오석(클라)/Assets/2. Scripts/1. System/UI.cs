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

    [Header("Panel")]
    [SerializeField] private GameObject deadPanel;
    [SerializeField] private GameObject clearPanel;

    [Header("Ammo")]
    [SerializeField] private Weapon weapon;
    [SerializeField] private TextMeshProUGUI ammoTxt;

    private Bullet bullet;

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

            //Ammo
            ammoTxt.text = weapon.currentAmmo + " / " + weapon.maxAmmo + " | " + weapon.reserveAmmo;
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
            weapon.IncreaseMeleeDamage(1);
            p.statPoint -= 1;
        }
    }

    public void UpgradeRangeDamage()
    {
        if (p.statPoint > 0)
        {
            bullet.IncreaseRangeDamage(1);
            p.statPoint -= 1;
        }
    }
}
