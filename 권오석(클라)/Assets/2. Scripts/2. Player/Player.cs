using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Player : MonoBehaviour
{
    [Title("Weapons")]
    [SerializeField, TabGroup("Weapons")] private bool[] hasWeapons;
    [SerializeField, TabGroup("Weapons")] private GameObject[] weapons;
    [SerializeField, TabGroup("Weapons")] private GameObject subWeapon;
    [SerializeField, TabGroup("Weapons")] public BoxCollider meleeArea;

    [Title("Player Stats")]
    [TabGroup("Stats", "Health")] public int maxHP;
    [TabGroup("Stats", "Health")] public int curHP;
    [TabGroup("Stats", "Experience")] public int level;
    [TabGroup("Stats", "Experience")] public int curExp;
    [TabGroup("Stats", "Experience")] public int maxExp;
    [TabGroup("Stats", "Experience")] public int statPoint;
    private float speed;
    private int healAmount;
    private int expAmount;

    [Title("Components")]
    [SerializeField, TabGroup("Components", "Component")] Camera followCamera;

    // Move
    private bool sprint;
    private bool isDash;
    private bool onDash;
    private float dashTime;
    private Vector3 moveVec;
    private Vector3 dashVec;

    // Weapon
    private GameObject nearObject;
    private Weapon equipWeapon;
    private int equipWeaponIndex = -1;
    private bool interaction;
    private bool swapWeapon1;
    private bool swapWeapon2;

    // Attack
    private bool atk;
    private float fireDelay;
    private bool isFireReady = true;
    private bool isAtkMoving = false;
    private Vector3 atkPosition;
    private float atkMoveSpeed = 5f;
    public float atkResetTime = 4f;
    public int atkCombo = 0;

    // Other
    private Animator animator;
    private Rigidbody rb;
    private bool toWall; // �� �浹 Ȯ��

    // Animator�� Rigidbody ������Ʈ�� �ʱ�ȭ
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // �÷��̾��� �ʱ� ���¿� ���� ���� ����
    void Start()
    {
        maxHP = 30;
        curHP = maxHP;

        speed = 3;

        level = 1;
        curExp = 0;
        maxExp = 100;
        statPoint = 10;

        healAmount = 20;
        expAmount = 30;
    }

    // �÷��̾��� �̵�, ȸ��, �뽬, ����, ���� ��ü �� ��ȣ�ۿ��� ó��
    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        Move();
        Turn();
        Dash();

        Attack();

        Swap();
        Interaction();

        if (isAtkMoving) // ���� ���� �� ������ ó��
        {
            transform.position = Vector3.Lerp(transform.position, atkPosition, atkMoveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, atkPosition) < 0.1f)
            {
                isAtkMoving = false;
            }
        }
    }

    // �÷��̾��� �̵� ó��
    private void Move()
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

    // �÷��̾ ���콺 ��ġ�� �̵� �������� ȸ��
    private void Turn()
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

    // �뽬 ���� ó��
    private void Dash()
    {
        if (Input.GetButtonDown("Dash") && moveVec != Vector3.zero && !onDash && !toWall)
        {
            Vector3 dashDirection = transform.forward;
            float dashDistance = 2.5f; // �뽬 �Ÿ�

            StartCoroutine(Dash(dashDirection, dashDistance));

            animator.SetTrigger("Forward-Dash");
            isDash = true;
            onDash = true;

            Invoke("EndDash", 0.5f); // �뽬 ��Ÿ��
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

    // �뽬 �ڷ�ƾ ó��
    private IEnumerator Dash(Vector3 direction, float distance)
    {
        float dashTime = 0.3f; // �뽬 ���� �ð�
        float elapsedTime = 0f;

        float dashSpeed = distance / dashTime;

        while (elapsedTime < dashTime)
        {
            transform.position += direction * dashSpeed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    // �뽬 ���� ó��
    private void EndDash()
    {
        onDash = false;
    }

    // ���� ó��
    private void Attack()
    {
        atk = Input.GetButtonDown("Fire1");

        if (equipWeapon == null)
            return;
        if (equipWeapon.currentAmmo == 0)
        {
            Debug.Log("Reloading...");
            return;
        }

        if (atkCombo >= 4 || atkResetTime <= 0)
        {
            atkCombo = 0;
            atkResetTime = 4f;
        }
        if (atkCombo >= 1)
        {
            atkResetTime -= Time.deltaTime;
        }

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.atkSpeed < fireDelay;

        if (atk && isFireReady && !isDash)
        {
            equipWeapon.Use();

            if (equipWeapon.type == Weapon.Type.Melee)
            {
                animator.SetTrigger("Melee-Attack");
                animator.SetFloat("MeleeCombo", atkCombo);
            }
            else if (equipWeapon.type == Weapon.Type.Range)
            {
                animator.SetTrigger("Range-Attack");
            }
            atkCombo++;
            fireDelay = 0;
            atkResetTime = 4f;
        }
    }

    // ������ �ִϸ��̼� Ʈ����
    public void Reload()
    {
        animator.SetTrigger("doReload");
    }

    // ���� ���� �� ������ ó��
    public void ActiveMeleeAttack()
    {
        isAtkMoving = true;

        atkPosition = transform.position + transform.forward * 0.7f;
    }

    // ���� ��ü ó��
    private void Swap()
    {
        swapWeapon1 = Input.GetButtonDown("Swap1");
        swapWeapon2 = Input.GetButtonDown("Swap2");

        if (swapWeapon1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (swapWeapon2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;

        int weaponIndex = -1;
        if (swapWeapon1) weaponIndex = 0; // ��������
        if (swapWeapon2) weaponIndex = 1; // ����

        if ((swapWeapon1 || swapWeapon2) && !isDash)
        {
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            subWeapon.SetActive(swapWeapon1);
            UI.Instance.curWeapon[0].SetActive(swapWeapon1);
            UI.Instance.curWeapon[1].SetActive(swapWeapon2);
        }
    }

    // ������ �ݱ� ó��
    private void Interaction()
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

    // ������ ���� �� ȸ�� ����
    private void StopRotation()
    {
        rb.angularVelocity = Vector3.zero;
    }

    // �� �浹 Ȯ��
    private void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 0.7f, Color.green);
        toWall = Physics.Raycast(transform.position, transform.forward, 0.7f, LayerMask.GetMask("Wall"));
    }

    // �浹 ó��
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
            Debug.Log(healAmount + "|" + curHP + "/" + maxHP);
            Destroy(other.gameObject);
        }
        else if (other.tag == "Exp")
        {
            RestoreExp(expAmount);
            Debug.Log(expAmount + "|" + curExp + "/" + maxExp);
            Destroy(other.gameObject);
        }
    }

    // ü�� ȸ�� ó��
    private void RestoreHealth(int amount)
    {
        curHP += amount;
        if (curHP > maxHP)
        {
            curHP = maxHP;
        }
        Debug.Log("Health Restored. Current Health: " + curHP);
    }

    // ����ġ ȸ�� ó��
    private void RestoreExp(int amount)
    {
        curExp += amount;
        if (curExp >= maxExp)
        {
            curExp -= maxExp;
            LevelUp();
        }
        Debug.Log("Exp Restored. Current Exp: " + curExp);
    }

    // ���� �� ó��
    private void LevelUp()
    {
        level++;
        maxExp = NextLevelExp();
        maxHP += 2;
        statPoint += 1;
        curHP = maxHP;
    }

    // ���� ������ ����ġ ���
    private int NextLevelExp()
    {
        return Mathf.RoundToInt(maxExp * 1.2f);
    }

    // ���� ó��
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

    // ��� ó��
    public void Dead()
    {
        UI.Instance.DeadPanel();
    }

    // Ʈ���� �浹 �� ó��
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Ammo")
            nearObject = other.gameObject;
    }

    // Ʈ���� �浹 ���� �� ó��
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Ammo")
            nearObject = null;
    }
}
