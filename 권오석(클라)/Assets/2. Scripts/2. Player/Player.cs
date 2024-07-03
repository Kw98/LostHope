using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private GameObject subWeapon;
    [SerializeField] private bool[] hasWeapons;
    [SerializeField] Camera followCamera;

    [Header("Player")]
    public int maxHP;
    public int curHP;
    private float speed;
    public int level;
    public int curExp;
    public int maxExp;
    public int statPoint;
    private int healAmount;

    //Move
    private bool sprint;
    private bool isDash;
    private bool onDash;
    private float dashTime;
    private Vector3 moveVec;
    private Vector3 dashVec;

    //Weapon
    private GameObject nearObject;
    private Weapon equipWeapon;
    private int equipWeaponIndex = -1;
    private bool interaction;
    private bool swapWeapon1;
    private bool swapWeapon2;

    //Attack
    private bool atk;
    private float fireDelay;
    private bool isFireReady = true;
    private bool isAtkMoving = false;
    private Vector3 atkPosition;
    private float atkMoveSpeed = 5f;
    public BoxCollider meleeArea;
    //AtkCombe
    private int atkCombo;
    private Coroutine resetComboCoroutine;
    private bool isCheckInput = false;

    //Other
    private Animator animator;
    private Rigidbody rb;
    private bool toWall; // 벽 충돌확인

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        maxHP = 30;
        curHP = maxHP;

        speed = 3;

        level = 1;
        curExp = 0;
        maxExp = 100;
        statPoint = 0;

        healAmount = 20;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Turn();
        Dash();

        Attack();

        Swap();
        Interaction();

        if (isAtkMoving) // 근접 공격 시 전진성
        {
            transform.position = Vector3.Lerp(transform.position, atkPosition, atkMoveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, atkPosition) < 0.1f)
            {
                isAtkMoving = false;
            }
        }
    }

    public void GainExp(int amount)
    {
        curExp += amount;
        CheckLevelup();
    }

    private void CheckLevelup()
    {
        if (curExp >= maxExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        curExp = 0;
        maxExp += 50;
        statPoint += 3;
    }

    private void Move() // 이동
    {
        float hMove = Input.GetAxisRaw("Horizontal");
        float vMove = Input.GetAxisRaw("Vertical");
        sprint = Input.GetButton("Sprint");

        moveVec = new Vector3(hMove, 0, vMove).normalized;

        if (isDash)
            moveVec = dashVec;

        if (!isFireReady)
            return;

        if (!toWall)
            transform.position += moveVec * (sprint ? speed = 6 : speed) * Time.deltaTime;

        animator.SetBool("isWalk", moveVec != Vector3.zero);
        animator.SetBool("isSprint", sprint);
    }

    private void Turn() // 캐릭터 회전
    {
        transform.LookAt(transform.position + moveVec);

        if (Input.GetButton("Fire1"))
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, 100))
            {
                Vector3 vector = raycastHit.point - transform.position;
                vector.y = 0;
                transform.LookAt(transform.position + vector);
            }
        }
    }

    private void Dash() // 대쉬
    {
        if (Input.GetButtonDown("Dash") && moveVec != Vector3.zero && !onDash && !toWall)
        {
            Vector3 dashDirection = transform.forward;
            float dashDistance = 2.5f; // 대쉬 거리

            StartCoroutine(Dash(dashDirection, dashDistance));

            animator.SetTrigger("Forward-Dash");
            isDash = true;
            onDash = true;

            Invoke("EndDash", 0.5f); // 대쉬 쿨타임
        }

        if (dashTime <= 0)
            speed = 3;
        else
        {
            dashTime -= Time.deltaTime;
            speed = 15;
        }
        isDash = false;
    }

    private IEnumerator Dash(Vector3 direction, float distance) // 대쉬
    {
        float dashTime = 0.3f; // 대쉬 지속 시간
        float elapsedTime = 0f;

        float dashSpeed = distance / dashTime;

        while (elapsedTime < dashTime)
        {
            transform.position += direction * dashSpeed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void EndDash()
    {
        onDash = false;
    }

    private void Attack() // 공격
    {
        atk = Input.GetButtonDown("Fire1");

        if (equipWeapon == null)
            return;
        if (equipWeapon.currentAmmo == 0)
        {
            Debug.Log("Reloading...");
            return;
        }

        if (atkCombo >= 4)
            atkCombo = 0;

        // 일정 시간 후 콤보 초기화
        if (resetComboCoroutine != null)
        {
            StopCoroutine(resetComboCoroutine);
        }
        resetComboCoroutine = StartCoroutine(ResetComboAfterDelay());

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.atkSpeed < fireDelay;

        if (atk && isFireReady && !isDash)
        {
            equipWeapon.Use();

            if (equipWeapon.type == Weapon.Type.Melee)
            {
                animator.SetTrigger("Melee-Attack");
                animator.SetInteger("MeleeCombo", atkCombo);
            }
            else if (equipWeapon.type == Weapon.Type.Range)
            {
                animator.SetTrigger("Range-Attack");
            }
            atkCombo++;
            fireDelay = 0;
        }
    }

    public void OnAtkCollider()
    {
        meleeArea.enabled = true;
    }

    public void OffAtkCollider()
    {
        meleeArea.enabled = false;
    }


    public void Reload()
    {
        animator.SetTrigger("doReload");
    }

    private IEnumerator ResetComboAfterDelay()
    {
        yield return new WaitForSeconds(2f); // 콤보 초기화 시간
        atkCombo = 0;
        animator.SetInteger("MeleeCombo", atkCombo);
    }

    private IEnumerator CheckInputDuringAnimation()
    {
        isCheckInput = true;
        yield return new WaitForSeconds(0.5f); // 입력 시간
        if (atkCombo >= 1)
        {
            Attack();
        }
        isCheckInput = false;
    }

    public void CheckAttackInput()
    {
        StartCoroutine(CheckInputDuringAnimation());
    }

    public void ActiveMeleeAttack() // 근접 공격 시 전진성
    {
        isAtkMoving = true;

        atkPosition = transform.position + transform.forward * 0.7f;
    }

    private void Swap() // 무기 교체
    {
        swapWeapon1 = Input.GetButtonDown("Swap1");
        swapWeapon2 = Input.GetButtonDown("Swap2");

        if (swapWeapon1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (swapWeapon2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;

        int weaponIndex = -1;
        if (swapWeapon1) weaponIndex = 0; // 근접무기
        if (swapWeapon2) weaponIndex = 1; // 권총

        if ((swapWeapon1 || swapWeapon2) && !isDash)
        {
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            if (swapWeapon1 == true) // 보조무기 활성화
                subWeapon.SetActive(true);
            else
                subWeapon.SetActive(false);
        }
    }

    private void Interaction() // 아이템 줍기
    {
        interaction = Input.GetButtonDown("Interaction");

        if (interaction && nearObject != null && !isDash)
        {
            Item item = nearObject.GetComponent<Item>();
            if (item != null)
            {
                if (nearObject.tag == "Weapon")
                {
                    int weaponIndex = item.value;
                    hasWeapons[weaponIndex] = true;

                    Destroy(nearObject);
                }
                else if (nearObject.tag == "Ammo" && equipWeapon != null && equipWeaponIndex == 1 && hasWeapons[1])
                {
                    int ammoAmount = item.value;
                    equipWeapon.AddReserveAmmo(ammoAmount);

                    Destroy(nearObject);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        StopRotation();
        StopToWall();
    }

    private void StopRotation() // 아이템 습득 후 회전 방지
    {
        rb.angularVelocity = Vector3.zero;
    }

    private void StopToWall() // 벽 충돌 확인
    {
        Debug.DrawRay(transform.position, transform.forward * 0.7f, Color.green);
        toWall = Physics.Raycast(transform.position, transform.forward, 0.7f, LayerMask.GetMask("Wall"));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "MonsterAtk" || other.tag == "Bullet")
        {
            if (other.tag == "MonsterAtk")
            {
                Monster monster = other.GetComponentInParent<Monster>();
                if (monster != null)
                {
                    int damage = monster.data.Power;
                    curHP -= damage;
                    Debug.Log("Player HP : " + curHP);
                    StartCoroutine(OnDamage());
                }
            }
            else if (other.tag == "Bullet")
            {
                Bullet bullet = other.GetComponent<Bullet>();
                if (bullet != null)
                {
                    curHP -= bullet.damage;
                    Debug.Log("Player HP : " + curHP);
                    StartCoroutine(OnDamage());
                }
            }
        }

        if (other.tag == "HealthPotion")
        {
            RestoreHealth(healAmount);
            Destroy(other.gameObject);
        }
    }

    void RestoreHealth(int amount)
    {
        curHP += amount;
        if (curHP > maxHP)
        {
            curHP = maxHP;
        }
        Debug.Log("Health Restored. Current Health: " + curHP);
    }

    public IEnumerator OnDamage()
    {
        if (curHP > 0)
        {
            animator.SetTrigger("GetHit");

            yield return new WaitForSeconds(1f);
        }
        else
        {
            animator.SetTrigger("onDead");

            Destroy(gameObject, 1f);

            Dead();
        }
    }

    public void Dead()
    {
        UI.Instance.DeadPanel();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Ammo")
            nearObject = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Ammo")
            nearObject = null;
    }
}
