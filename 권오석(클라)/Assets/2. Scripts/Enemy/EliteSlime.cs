using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteSlime : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public override void Init()
    {
        chaseDistance = 6;
        data.MaxHP = 20;
        data.CurHP = data.MaxHP;

        base.Init();
    }
}
