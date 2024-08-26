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

    // �ʱ�ȭ �� ���� �� ȣ��Ǵ� �Լ�
    void Start()
    {
        Init();
        data.MaxHP = data.CurHP;
    }

    // ���͸� �ʱ�ȭ�ϴ� �Լ�
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

    // �� ������ ȣ��Ǹ�, ������ ���¸� ������Ʈ�ϴ� �Լ�
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

            // �÷��̾ ���� ���� ���� �ִ��� Ȯ��
            if (distanceToPlayer <= chaseDistance)
            {
                target = GameManager.Instance.P.transform;
                bossUI.gameObject.SetActive(true);
            }
        }
        else
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // Ÿ���� ���� ������ ����� Ÿ���� ����
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

        // ���� ���� ���� �� ���͸� ��ǥ ��ġ�� �̵���Ű�� �ڵ�
        if (isAtkMoving)
        {
            transform.position = Vector3.Lerp(transform.position, atkPosition, atkMoveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, atkPosition) < 0.1f)
            {
                isAtkMoving = false;
            }
        }
    }

    // ���Ͱ� �ൿ�� �����ϴ� �ڷ�ƾ
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

    // ���Ͱ� ������ �������� �̵��ϴ� �ڷ�ƾ
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

    // ������ ������ �����ϴ� �ڷ�ƾ
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

    // ������ ���⸦ �����ϴ� �Լ�
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

    // ������ ���� ������ �����ϴ� �ڷ�ƾ
    IEnumerator MeleeAtk()
    {
        ChangeWeapon(0);
        animator.SetTrigger("doMeleeAtk");
        yield return new WaitForSeconds(2f);
    }

    // ������ ���Ÿ� ������ �����ϴ� �ڷ�ƾ
    IEnumerator RangeAtk()
    {
        ChangeWeapon(1);
        animator.SetTrigger("doRangeAtk");
        yield return new WaitForSeconds(3f);
    }

    // ������ �뽬 ������ �����ϴ� �ڷ�ƾ
    IEnumerator DashAtk()
    {
        ChangeWeapon(0);
        dashVec = (target.position - transform.position).normalized;

        animator.SetTrigger("doDashAtk");
        yield return new WaitForSeconds(3f);
    }

    // ���� ���� �� �ݶ��̴��� Ȱ��ȭ�ϴ� �Լ�
    public void EnableAttackCollider()
    {
        meleeArea.enabled = true;
    }

    // ���� ���� ���� �� �ݶ��̴��� ��Ȱ��ȭ�ϴ� �Լ�
    public void DisableAttackCollider()
    {
        meleeArea.enabled = false;
    }

    // ���� ���� �� ���͸� ��ǥ ��ġ�� ������Ű�� �Լ�
    public void ActiveMeleeAttack()
    {
        isAtkMoving = true;
        atkPosition = transform.position + transform.forward * 1f;
    }

    // �뽬 ���� �� ���͸� ��ǥ ��ġ�� ������Ű�� �Լ�
    public void ActiveDashAttack()
    {
        isAtkMoving = true;
        atkPosition = transform.position + transform.forward * 7f;
    }

    // ���Ÿ� ���� �� �Ѿ��� �߻��ϴ� �Լ�
    public void FireBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = firePoint.forward * 20f; // �Ѿ��� �ӵ�
    }

    // ���� UI�� ������Ʈ�ϴ� �Լ�
    private void BossUI()
    {
        float hpFillAmount = (float)data.CurHP / data.MaxHP;
        curHPImage.fillAmount = hpFillAmount;
    }
}