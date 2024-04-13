using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private float speed;
    [SerializeField] private float damege;
    [SerializeField] private float meleeAtkRange;

    private bool sprint;

    private bool isDodge;
    private bool onDodge;
    private float dodgeTime;

    private Vector3 moveVec;
    private Vector3 dodgeVec;

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
        Dodge();

        if (Input.GetMouseButtonDown(0))
        {
            MeleeAttack();
        }
    }

    private void Move()
    {
        float hMove = Input.GetAxisRaw("Horizontal");
        float vMove = Input.GetAxisRaw("Vertical");
        sprint = Input.GetButton("Sprint");

        if (isDodge)
            moveVec = dodgeVec;

        moveVec = new Vector3(hMove, 0, vMove).normalized;

        transform.position += moveVec * speed * Time.deltaTime;

        animator.SetBool("isWalk", moveVec != Vector3.zero);
        animator.SetBool("isSprint", sprint);

        transform.LookAt(transform.position + moveVec);
    }

    private void Dodge()
    {
        if (Input.GetButtonDown("Dodge") && moveVec != Vector3.zero && !onDodge)
        {
            animator.SetTrigger("Forward-Dodge");
            isDodge = true;
            onDodge = true;

            Invoke("NextDodge", 0.5f);//Dodge Cooltime
        }

        if (dodgeTime <= 0)
        {
            speed = 5;
            if (isDodge)
                dodgeTime = 0.25f;
        }
        else
        {
            dodgeTime -= Time.deltaTime;
            speed = 15;
        }

        isDodge = false;
    }

    private void NextDodge()
    {
        onDodge = false;
    }

    private void MeleeAttack()
    {

    }
}
