using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlime : Monster
{
    [SerializeField] private int dataIndex;

    private Vector3 dashVec;

    private bool isAtkMoving = false;
    private Vector3 atkPosition;
    private float atkMoveSpeed = 5f;

    private bool isThinking;

    [Header("Weapon")]
    [SerializeField] private GameObject[] mainWeapons;
    [SerializeField] private GameObject subWeapon;

    private int currentWeaponIndex;

    private void ChangeWeapon(int weaponIndex)
    {
        if (currentWeaponIndex != weaponIndex)
        {
            mainWeapons[currentWeaponIndex].SetActive(false);

            if (currentWeaponIndex == 0 && subWeapon != null)
            {
                subWeapon.SetActive(false);
            }

            mainWeapons[weaponIndex].SetActive(true);

            if (weaponIndex == 0 && subWeapon != null)
            {
                subWeapon.SetActive(true);
            }

            currentWeaponIndex = weaponIndex;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public override void Init()
    {
        chaseDistance = 13;
        data.CurHP = 50;

        JsonData.MonsterJsonData jData = JsonData.Instance.mj.monster[dataIndex];

        data.Power = jData.power;
        data.AtkDelay = jData.atkdelay;
        data.Speed = jData.speed;

        base.Init();

        currentWeaponIndex = 0;
        for (int i = 0; i < mainWeapons.Length; i++)
        {
            mainWeapons[i].SetActive(i == currentWeaponIndex);
        }
        if (subWeapon != null)
        {
            subWeapon.SetActive(currentWeaponIndex == 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }

        if (target == null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position,
                                                GameManager.Instance.P.transform.position);

            // ���� ���� �� Ÿ�� ����
            if (distanceToPlayer <= chaseDistance)
            {
                target = GameManager.Instance.P.transform;
            }
        }
        else
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // ��� ��� Ÿ�� null
            if (distanceToTarget > chaseDistance)
            {
                target = null;
            }
            else
            {
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

                if (!isThinking)
                {
                    StartCoroutine(Think());
                }
            }
        }

        if (isAtkMoving) // ���� ���� �� ������
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
        isThinking = true;

        int random = Random.Range(0, 2);
        switch (random)
        {
            case 0:
                StartCoroutine(RandomMovement());
                break;

            case 1:
                StartCoroutine(Attack());
                break;
        }

        yield return new WaitForSeconds(2f);
        isThinking = false;
    }

    IEnumerator RandomMovement()
    {
        float randomDuration = Random.Range(1f, 2f);
        float elapsedTime = 0f;

        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + randomDirection * data.Speed * randomDuration;

        animator.SetBool("isWalk", true);

        while (elapsedTime < randomDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / randomDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        animator.SetBool("isWalk", false);
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
        else
        {
            StartCoroutine(RandomMovement());
        }

        yield return new WaitForSeconds(3f);
    }

    IEnumerator MeleeAtk()
    {
        ChangeWeapon(0);
        animator.SetTrigger("doMeleeAtk");
        yield return new WaitForSeconds(2f);
    }

    IEnumerator RangeAtk()
    {
        ChangeWeapon(1);
        animator.SetTrigger("doRangeAtk");
        yield return new WaitForSeconds(3f);
    }

    IEnumerator DashAtk()
    {
        ChangeWeapon(0);
        dashVec = (target.position - transform.position).normalized;

        animator.SetTrigger("doDashAtk");
        yield return new WaitForSeconds(3f);
    }

    public void ActiveMeleeAttack() // ���� ���� �� ������
    {
        isAtkMoving = true;

        atkPosition = transform.position + transform.forward * 1f;
    }

    public void ActiveDashAttack() // �뽬 ���� �� ������
    {
        isAtkMoving = true;

        atkPosition = transform.position + transform.forward * 7f;
    }
}
