using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//무기스왑 액션 포함 하여 무기에 따른 공격이 나가야함 + 타켓을 안따라옴 ㅅㅂ
public class KingSlime : Monster
{
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private GameObject subWeapon;
    [SerializeField] private bool[] hasWeapons;

    [SerializeField] private int dataIndex;

    private bool isLook;
    private Vector3 lookVec;
    private Vector3 dashVec;

    private bool isAtkMoving = false;
    private Vector3 atkPosition;
    private float atkMoveSpeed = 5f;
    private bool isDash;


    // Start is called before the first frame update
    void Start()
    {
        Init();
        StartCoroutine(Think());
        isLook = true;
        nav.isStopped = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }

        if (isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 0.5f;
            transform.LookAt(target.position + lookVec);
        }
        else
            nav.SetDestination(dashVec);

        if (isAtkMoving) // 근접 공격 시 전진성
        {
            transform.position = Vector3.Lerp(transform.position, atkPosition, atkMoveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, atkPosition) < 0.1f)
            {
                isAtkMoving = false;
            }
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

        base.Init();
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.5f);

        int ranAction = Random.Range(0, 5);

        switch (ranAction)
        {
            case 0:

            case 1:
                StartCoroutine(MeleeAtk());
                break;
            case 2:

            case 3:
                StartCoroutine(RangeAtk());
                break;
            case 4:
                StartCoroutine(DashAtk());
                break;
        }
    }

    IEnumerator MeleeAtk()
    {
        animator.SetTrigger("doMeleeAtk");
        yield return new WaitForSeconds(2f);

        StartCoroutine(Think());
    }
    IEnumerator RangeAtk()
    {
        animator.SetTrigger("doRangeAtk");
        yield return new WaitForSeconds(3f);

        StartCoroutine(Think());
    }
    IEnumerator DashAtk()
    {
        dashVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        isDash = true;

        animator.SetTrigger("doDashAtk");
        yield return new WaitForSeconds(3f);

        isLook = true;
        nav.isStopped = true;
        isDash = false;

        StartCoroutine(Think());
    }

    public void ActiveMeleeAttack() // 근접 공격 시 전진성
    {
        isAtkMoving = true;

        atkPosition = transform.position + transform.forward * 0.7f;
    }
}
