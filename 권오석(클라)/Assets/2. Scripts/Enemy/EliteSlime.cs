using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteSlime : Monster
{
    [SerializeField] private int dataIndex;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public override void Init()
    {
        chaseDistance = 6;
        data.CurHP = 20;

        JsonData.MonsterJsonData jData = JsonData.Instance.mj.monster[dataIndex];

        data.Power = jData.power;
        data.AtkDelay = jData.atkdelay;
        data.Speed = jData.speed;

        base.Init();
    }
}
