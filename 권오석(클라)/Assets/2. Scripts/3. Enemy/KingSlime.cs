using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlime : Monster
{
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private GameObject subWeapon;
    [SerializeField] private bool[] hasWeapons;
    [SerializeField] private GameObject currentWeapon;

    [SerializeField] private int dataIndex;

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
        isMove = true;

        SwapWeapon(0);
    }

    public override void Init()
    {
        chaseDistance = 10;
        data.CurHP = 50;

        JsonData.MonsterJsonData jData = JsonData.Instance.mj.monster[dataIndex];

        data.Power = jData.power;
        data.AtkDelay = jData.atkdelay;
        data.Speed = jData.speed;

        base.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }

        if (isMove)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Vector3 movement = direction * data.Speed * Time.deltaTime;
            transform.position += movement;
            transform.LookAt(target.position);

            animator.SetBool("isWalk", true);
        }

        if (isAtkMoving) // 근접 공격 시 전진성
        {
            transform.position = Vector3.Lerp(transform.position, atkPosition, atkMoveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, atkPosition) < 0.1f)
            {
                isAtkMoving = false;
            }
        }
    }

    IEnumerator Think()
    {
        int random = Random.Range(0, 3);
        switch (random)
        {
            case 0:
                isMove = false;
                break;

            case 1:
                isMove = true;
                StartCoroutine(RandomMovement());
                break;

            case 2:
                StartCoroutine(Attack());
                break;
        }

        yield return new WaitForSeconds(2f);
    }
    
    IEnumerator RandomMovement()
    {
        // 무작위로 이동
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        Vector3 targetPosition = transform.position + randomDirection * 2f; // 이동 거리
        transform.position = Vector3.Lerp(transform.position, targetPosition, data.Speed * Time.deltaTime);
        transform.LookAt(target.position);

        animator.SetBool("isWalk", true);

        yield return new WaitForSeconds(2f);
    }

    IEnumerator Attack()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget < 4f)
        {
            StartCoroutine(MeleeAtk());
        }
        else if (distanceToTarget >= 4f && distanceToTarget < 6f)
        {
            StartCoroutine(DashAtk());
        }
        else if (distanceToTarget >= 6f && distanceToTarget <= 10f)
        {
            StartCoroutine(RangeAtk());
        }

        yield return new WaitForSeconds(3f);
    }

    IEnumerator MeleeAtk()
    {
        isMove = false;
        animator.SetBool("isWalk", false);

        SwapWeapon(0);

        animator.SetTrigger("doMeleeAtk");
        yield return new WaitForSeconds(2f);

        isMove = true;

        StartCoroutine(Think());
    }

    IEnumerator RangeAtk()
    {
        isMove = false;
        animator.SetBool("isWalk", false);

        SwapWeapon(1);

        animator.SetTrigger("doRangeAtk");
        yield return new WaitForSeconds(3f);

        isMove = true;

        StartCoroutine(Think());
    }

    IEnumerator DashAtk()
    {
        isMove = false;
        animator.SetBool("isWalk", false);

        SwapWeapon(0);

        dashVec = (target.position - transform.position).normalized;

        isDash = true;

        animator.SetTrigger("doDashAtk");
        yield return new WaitForSeconds(3f);

        isDash = false;
        isMove = true;

        StartCoroutine(Think());
    }

    public void SwapWeapon(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < weapons.Length && hasWeapons[weaponIndex])
        {
            if (currentWeapon != null)
            {
                currentWeapon.SetActive(false);
            }

            currentWeapon = weapons[weaponIndex];
            currentWeapon.SetActive(true);
        }
    }

    public void ActiveMeleeAttack() // 근접 공격 시 전진성
    {
        isAtkMoving = true;

        atkPosition = transform.position + transform.forward * 0.7f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
        }
    }
}
