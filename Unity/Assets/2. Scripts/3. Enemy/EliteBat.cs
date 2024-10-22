using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteBat : Monster
{
    [SerializeField] private int dataIndex;
    // Start is called before the first frame update
    void Start()
    {
        Init();
        data.MaxHP = data.CurHP;
    }

    public override void Init()
    {
        chaseDistance = 7;
        data.CurHP = 16;

        JsonData.MonsterJsonData jData = JsonData.Instance.mj.monster[dataIndex];

        data.Power = jData.power;
        data.AtkDelay = jData.atkdelay;
        data.Speed = jData.speed;

        base.Init();
    }
}
