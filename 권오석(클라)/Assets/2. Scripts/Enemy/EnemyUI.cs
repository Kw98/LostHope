using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUI : MonoBehaviour
{
    public Enemy enemy;

    [Header("HP")]
    [SerializeField] private Image curHPImage;

    private Transform camera;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main.transform;

        enemy = GetComponentInParent<Enemy>();

        Transform child = transform.GetChild(0).GetChild(0);
        curHPImage = child.GetComponent<Image>();

        if (enemy != null)
        {
            UpdateUI();
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + camera.rotation * Vector3.forward,
                            camera.rotation * Vector3.up);

        if (enemy != null)
        {
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        //HP
        float hpFillAmount = (float)enemy.data.CurHP / enemy.data.MaxHP;
        curHPImage.fillAmount = hpFillAmount;
    }
}
