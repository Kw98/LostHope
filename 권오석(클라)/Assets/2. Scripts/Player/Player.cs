using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private float speed;
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private bool[] hasWeapons;

    private Define.PlayerData data = new Define.PlayerData();

    //Move
    private bool sprint;
    private bool isDash;
    private bool onDash;
    private float dashTime;
    private Vector3 moveVec;
    private Vector3 dashVec;
    public Camera followCamera;

    //Weapon
    private GameObject nearObject;
    private Weapon equipWeapon;
    private int equipWeaponIndex = -1;
    private bool interaction;
    private bool swapWeapon1;
    private bool swapWeapon2;
    private bool isSwap;

    //Attack
    private bool atk;
    private float fireDelay;
    private bool isFireReady = true;
    private bool isAtkMoving = false;
    private Vector3 atkPosition;
    private float atkMoveSpeed = 5f;

    //Other
    private Animator animator;
    private Rigidbody rb;
    private bool toWall; // 벽 충돌확인

    public float SetCurHP
    {
        get { return data.CurHP; }
        set
        {
            data.CurHP = value;
            //UI.Instance.RefreshHP(hpImage);

            // Player Dead
            //if (data.CurHP <= 0 && state != PlayerState.Dead)
            //{
            //    state = PlayerState.Dead;
            //    sa.SetSprite(dead, 0.2f, Dead, 1f);
            //}
        }
    }

    public float SetMaxHP
    {
        get { return data.MaxHP; }
        set
        {
            data.MaxHP = value;
        }
    }
    public float SetExp
    {
        get { return data.Exp; }
        set
        {
            data.Exp = value;
            //UI.Instance.RefreshExp();
        }
    }

    public float SetMaxExp
    {
        get { return data.MaxExp; }
        set { data.MaxExp = value; }
    }


    public int SetLevel
    {
        get { return data.Level; }
        set
        {
            data.Level = value;
            //UI.Instance.RefreshLevel();
        }
    }

    public float SetPower
    {
        get { return data.Power; }
        set { data.Power = value; }
    }

    public float SetSpeed
    {
        get { return data.Speed; }
        set { data.Speed = value; }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        //Player State
        SetMaxHP = SetCurHP = 100;
        SetMaxExp = 100;

        SetExp = 0;
        SetLevel = 1;

        SetPower = 5;
        SetSpeed = 3;
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

    private void Move() // 이동
    {
        float hMove = Input.GetAxisRaw("Horizontal");
        float vMove = Input.GetAxisRaw("Vertical");
        sprint = Input.GetButton("Sprint");

        moveVec = new Vector3(hMove, 0, vMove).normalized;

        if (isDash)
            moveVec = dashVec;

        if (isSwap || !isFireReady)
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
        if (Input.GetButtonDown("Dash") && moveVec != Vector3.zero && !onDash && !isSwap && !toWall)
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

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.atkSpeed < fireDelay;

        if (atk && isFireReady && !isDash && !isSwap)
        {
            equipWeapon.Use();
            animator.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "Melee-Attack" : "Range-Attack");
            fireDelay = 0;
        }
    }
    public void ActiveMeleeAttack() // 근접 공격 시 전진성
    {
        isAtkMoving = true;

        atkPosition = transform.position + transform.forward * 0.5f;
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
        if (swapWeapon1) weaponIndex = 0;
        if (swapWeapon2) weaponIndex = 1;

        if ((swapWeapon1 || swapWeapon2) && !isDash && !isSwap)
        {
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            animator.SetTrigger("doSwap");
            isSwap = true;

            Invoke("EndSwap", 1.3f);
        }
    }

    private void EndSwap()
    {
        isSwap = false;
    }

    private void Interaction() // 아이템 줍기 (키 사용 x 자동습득)
    {
        interaction = Input.GetButtonDown("Interaction");

        if (interaction && nearObject != null && !isDash)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
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

    private void FixedUpdate()
    {
        StopRotation();
        StopToWall();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;

        Debug.Log(nearObject.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}
