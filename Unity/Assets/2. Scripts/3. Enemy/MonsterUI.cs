using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterUI : MonoBehaviour
{
    public Monster monster;

    [Header("HP")]
    [SerializeField] private Image curHPImage;

    private Transform _camera;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main.transform;

        monster = GetComponentInParent<Monster>();

        Transform child = transform.GetChild(0).GetChild(0);
        curHPImage = child.GetComponent<Image>();

        if (monster != null)
        {
            UpdateUI();
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + _camera.rotation * Vector3.forward,
                            _camera.rotation * Vector3.up);

        if (monster != null)
        {
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        //HP
        float hpFillAmount = (float)monster.data.CurHP / monster.data.MaxHP;
        curHPImage.fillAmount = hpFillAmount;
    }
}
