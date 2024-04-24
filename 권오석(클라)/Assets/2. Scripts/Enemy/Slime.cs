using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    [SerializeField] private int dataIndex;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public override void Init()
    {
        data.CurHP = 20;
        data.Exp = Define.stage * 50;
        data.MaxExp = 0;

        data.Speed = 3f;
        data.Power = 5f;
        data.HitDelayTime = 0.5f;
        data.AtkDistance = 1f;
        data.AtkDelay = 0.5f;

        base.Init();
    }
}
