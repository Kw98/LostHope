using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlime : Monster
{
    //private Weapon currentWeapon;
    //[SerializeField] private Transform rightHand;
    [SerializeField] private int dataIndex;

    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletParent;
    [SerializeField] private Transform bulletPos;

    private Vector3 lookVec;
    private Vector3 tauntVec;
    private bool isLook;

    

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(target.position + lookVec);
        }
    }
    public override void Init()
    {
        chaseDistance = 8;
        data.CurHP = 50;

        JsonData.MonsterJsonData jData = JsonData.Instance.mj.monster[dataIndex];

        data.Power = jData.power;
        data.AtkDelay = jData.atkdelay;
        data.Speed = jData.speed;

        //if (GameManager.Instance.P != null) // 플레이어 무기 복사
        //{
        //    Weapon weapon = GameManager.Instance.P.GetCurrentWeapon();
        //    if (weapon != null)
        //    {
        //        GameObject weaponObject = 
        //            Instantiate(weapon.gameObject, 
        //            rightHand.position, rightHand.rotation, rightHand);
        //        currentWeapon = weaponObject.GetComponent<Weapon>();
        //    }
        //}

        base.Init();
    }
}
