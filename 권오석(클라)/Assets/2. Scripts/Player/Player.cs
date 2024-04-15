using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private float speed;
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private bool[] hasWeapons;

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

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Dash();

        Attack();

        Swap();
        Interaction();

        if (isAtkMoving)
        {
            transform.position = Vector3.Lerp(transform.position, atkPosition, atkMoveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, atkPosition) < 0.1f)
            {
                isAtkMoving = false;
            }
        }
    }

    private void Move()
    {
        float hMove = Input.GetAxisRaw("Horizontal");
        float vMove = Input.GetAxisRaw("Vertical");
        sprint = Input.GetButton("Sprint");

        if (isSwap || !isFireReady)
            return;

        if (isDash)
            moveVec = dashVec;

        moveVec = new Vector3(hMove, 0, vMove).normalized;

        transform.position += moveVec * (sprint ? speed = 6 : speed) * Time.deltaTime;

        animator.SetBool("isWalk", moveVec != Vector3.zero);
        animator.SetBool("isSprint", sprint);

        transform.LookAt(transform.position + moveVec);
    }

    private void Dash()
    {
        if (Input.GetButtonDown("Dash") && moveVec != Vector3.zero && !onDash && !isSwap)
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

    private IEnumerator Dash(Vector3 direction, float distance)
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

    private void Attack()
    {
        atk = Input.GetButtonDown("Fire1");

        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.atkSpeed < fireDelay;

        if (atk && isFireReady && !isDash && !isSwap)
        {
            equipWeapon.Use();
            animator.SetTrigger("Melee-Attack");
            fireDelay = 0;
        }
    }
    public void ActiveMeleeAttack() //애니메이션 추가
    {
        isAtkMoving = true;

        atkPosition = transform.position + transform.forward * 0.5f;
    }

    private void Swap()
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

    private void Interaction()
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
