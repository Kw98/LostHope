using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public override void Init()
    {
        chaseDistance = 7;
        data.MaxHP = 8;
        data.CurHP = data.MaxHP;

        base.Init();
    }
}
