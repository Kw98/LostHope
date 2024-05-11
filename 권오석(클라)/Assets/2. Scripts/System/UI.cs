using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    public Player player;

    [Header("HP")]
    [SerializeField] private Image curHPImage;
    [SerializeField] private TextMeshProUGUI hpTxt;

    [Header("Level")]
    [SerializeField] private TextMeshProUGUI lvTxt;

    // Start is called before the first frame update
    void Start()
    {
        if (player != null)
        {
            UpdateUI();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        //HP
        float hpFillAmount = (float)player.curHP / player.maxHP;
        curHPImage.fillAmount = hpFillAmount;
        hpTxt.text = string.Format("{0}/{1}", player.curHP, player.maxHP);

        //LV
        string levelString = player.curLevel.ToString().PadLeft(2, '0');
        lvTxt.text = "LV " + levelString;
    }
}
