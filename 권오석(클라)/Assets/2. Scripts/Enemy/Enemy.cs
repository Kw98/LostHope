using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHP;
    [SerializeField] private int curHP;
    [SerializeField] private Transform target;
    [SerializeField] private BoxCollider meleeArea;
    [SerializeField] private float chaseDistance; // 플레이어 감지 범위

    public bool isChase;
    public bool isAtk;

    private Rigidbody rb;
    private Animator animator;
    private NavMeshAgent nav;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();

        //Invoke("ChaseStart", 2);
    }

    // Start is called before the first frame update
    void Start()
    {
        curHP = maxHP;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }

    //private void ChaseStart()
    //{
    //    isChase = true;
    //    animator.SetBool("isWalk", true);
    //}

    // Update is called once per frame
    void Update()
    {
        Targeting();
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < chaseDistance && !isChase)
        {
            isChase = true;
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
            
            animator.SetBool("isWalk", true);
            //ChaseStart();
        }
        else if (distanceToTarget >= chaseDistance && isChase) // 추적 중지
        {
            isChase = false;
            animator.SetBool("isWalk", false);
        }

        //if (nav.enabled)
        //{
        //    nav.SetDestination(target.position);
        //    nav.isStopped = !isChase;

        //    if (nav.velocity.magnitude > 0)
        //    {
        //        animator.SetBool("isWalk", true);
        //    }
        //    else
        //    {
        //        animator.SetBool("isWalk", false);
        //    }
        //}
    }

    private void FreezeVelocity()
    {
        if (isChase)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void Targeting()
    {
        float targetRadius = 1.0f;
        float targetRange = 0.5f;

        RaycastHit[] rayHits =
            Physics.SphereCastAll(transform.position
                                  , targetRadius
                                  , transform.forward
                                  , targetRange
                                  , LayerMask.GetMask("Player"));

        if (rayHits.Length > 0 && !isAtk)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAtk = true;
        animator.SetBool("isAtk1", true);

        yield return new WaitForSeconds(1f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(1f);

        isChase = true;
        isAtk = false;
        animator.SetBool("isAtk1", false);
    }

    private void FixedUpdate()
    {
        FreezeVelocity();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHP -= weapon.damage;
            Debug.Log("Melee : " + curHP);
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec));
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHP -= bullet.damage;
            Debug.Log("Range : " + curHP);
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec)
    {
        yield return new WaitForSeconds(0.1f);
        if (curHP > 0)
        {
            animator.SetTrigger("GetHit");
            transform.position = Vector3.zero;
            yield return new WaitForSeconds(1f);
        }
        else
        {
            gameObject.layer = 11;
            isChase = false;
            nav.enabled = false;
            animator.SetTrigger("onDead");

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rb.AddForce(reactVec * 10, ForceMode.Impulse);

            Destroy(gameObject, 1f);
        }
    }
}
