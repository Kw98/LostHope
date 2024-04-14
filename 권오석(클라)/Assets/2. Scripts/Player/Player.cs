using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private float speed;
    [SerializeField] private float maxAtkDelay;
    [SerializeField] private float atkDelay;
    private bool isAtk;

    private bool sprint;
    private bool isDodge;
    private bool onDodge;
    private float dodgeTime;

    private Vector3 moveVec;
    private Vector3 dodgeVec;

    private Animator animator;
    private Rigidbody rb;
    private Weapon weapon;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        weapon = GetComponent<Weapon>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Dodge();

        Attack();
    }

    private void Move()
    {
        float hMove = Input.GetAxisRaw("Horizontal");
        float vMove = Input.GetAxisRaw("Vertical");
        sprint = Input.GetButton("Sprint");

        if (isDodge)
            moveVec = dodgeVec;

        moveVec = new Vector3(hMove, 0, vMove).normalized;

        transform.position += moveVec * (sprint ? speed = 6 : speed) * Time.deltaTime;

        animator.SetBool("isWalk", moveVec != Vector3.zero);
        animator.SetBool("isSprint", sprint);

        transform.LookAt(transform.position + moveVec);
    }

    private void Dodge()
    {
        if (Input.GetButtonDown("Dodge") && moveVec != Vector3.zero && !onDodge)
        {
            Vector3 dodgeDirection = transform.forward;
            float dodgeDistance = 2.5f; // 회피할 거리

            // 회피 방향과 거리에 따라 플레이어를 이동시킵니다.
            StartCoroutine(Dash(dodgeDirection, dodgeDistance));

            animator.SetTrigger("Forward-Dodge");
            isDodge = true;
            onDodge = true;

            Invoke("NextDodge", 0.5f); // Dodge Cooltime
        }

        if (dodgeTime <= 0)
            speed = 3;
        else
        {
            dodgeTime -= Time.deltaTime;
            speed = 15;
        }
        isDodge = false;
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

    private void NextDodge()
    {
        onDodge = false;
    }

    private void Attack()
    {
        if (weapon == null)
            return;

        if (isDodge)
            return;

        atkDelay += Time.deltaTime;
        isAtk = weapon.atkSpeed < atkDelay;

        if (Input.GetMouseButton(0) && isAtk)
        {
            if (atkDelay >= maxAtkDelay)
            {
                //weapon.Use();
                animator.SetTrigger("Range-Attack");
                atkDelay = 0;
            }
        }
    }
}
