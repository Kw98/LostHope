using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class KingSlime : Monster
{
    private Vector3 dashVec;
    private bool isAtkMoving = false;
    private Vector3 atkPosition;
    private float atkMoveSpeed = 5f;

    private bool isThinking;
    private int currentWeaponIndex;

    [Title("KingSlime Setting")]
    [SerializeField] private int dataIndex;

    [SerializeField, TabGroup("Weapon", "Weapon")] private GameObject[] mainWeapons;
    [SerializeField, TabGroup("Weapon", "Weapon")] private GameObject subWeapon;

    [SerializeField, TabGroup("Bullet", "Bullet")] private GameObject bulletPrefab;
    [SerializeField, TabGroup("Bullet", "Bullet")] private Transform firePoint;

    [SerializeField, TabGroup("UI", "UI")] private Image curHPImage;
    [SerializeField, TabGroup("UI", "UI")] private GameObject bossUI;

    // 초기화 및 시작 시 호출되는 함수
    void Start()
    {
        Init();
        data.MaxHP = data.CurHP;
    }

    // 몬스터를 초기화하는 함수
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

    // 매 프레임 호출되며, 몬스터의 상태를 업데이트하는 함수
    void Update()
    {
        if (GameManager.Instance.P == null)
            return;

        if (isDead)
        {
            StopAllCoroutines();
            return;
        }
        BossUI();

        if (target == null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position,
                                                GameManager.Instance.P.transform.position);

            // 플레이어가 추적 범위 내에 있는지 확인
            if (distanceToPlayer <= chaseDistance)
            {
                target = GameManager.Instance.P.transform;
                bossUI.gameObject.SetActive(true);
            }
        }
        else
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // 타겟이 추적 범위를 벗어나면 타겟을 해제
            if (distanceToTarget > chaseDistance)
            {
                target = null;
                bossUI.gameObject.SetActive(false);
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

        // 근접 공격 중일 때 몬스터를 목표 위치로 이동시키는 코드
        if (isAtkMoving)
        {
            transform.position = Vector3.Lerp(transform.position, atkPosition, atkMoveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, atkPosition) < 0.1f)
            {
                isAtkMoving = false;
            }
        }
    }

    // 몬스터가 행동을 결정하는 코루틴
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

    // 몬스터가 랜덤한 방향으로 이동하는 코루틴
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

    // 몬스터의 공격을 결정하는 코루틴
    IEnumerator Attack()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget < 3f)
        {
            StartCoroutine(MeleeAtk());
        }
        else if (distanceToTarget >= 3f && distanceToTarget < 6f)
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

    // 몬스터의 무기를 변경하는 함수
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

    // 몬스터의 근접 공격을 실행하는 코루틴
    IEnumerator MeleeAtk()
    {
        ChangeWeapon(0);
        animator.SetTrigger("doMeleeAtk");
        yield return new WaitForSeconds(2f);
    }

    // 몬스터의 원거리 공격을 실행하는 코루틴
    IEnumerator RangeAtk()
    {
        ChangeWeapon(1);
        animator.SetTrigger("doRangeAtk");
        yield return new WaitForSeconds(3f);
    }

    // 몬스터의 대쉬 공격을 실행하는 코루틴
    IEnumerator DashAtk()
    {
        ChangeWeapon(0);
        dashVec = (target.position - transform.position).normalized;

        animator.SetTrigger("doDashAtk");
        yield return new WaitForSeconds(3f);
    }

    // 근접 공격 시 콜라이더를 활성화하는 함수
    public void EnableAttackCollider()
    {
        meleeArea.enabled = true;
    }

    // 근접 공격 종료 시 콜라이더를 비활성화하는 함수
    public void DisableAttackCollider()
    {
        meleeArea.enabled = false;
    }

    // 근접 공격 시 몬스터를 목표 위치로 전진시키는 함수
    public void ActiveMeleeAttack()
    {
        isAtkMoving = true;
        atkPosition = transform.position + transform.forward * 1f;
    }

    // 대쉬 공격 시 몬스터를 목표 위치로 전진시키는 함수
    public void ActiveDashAttack()
    {
        isAtkMoving = true;
        atkPosition = transform.position + transform.forward * 7f;
    }

    // 원거리 공격 시 총알을 발사하는 함수
    public void FireBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = firePoint.forward * 20f; // 총알의 속도
    }

    // 보스 UI를 업데이트하는 함수
    private void BossUI()
    {
        float hpFillAmount = (float)data.CurHP / data.MaxHP;
        curHPImage.fillAmount = hpFillAmount;
    }
}
