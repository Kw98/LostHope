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
    private bool isMove;

    private Rigidbody rb;
    private Animator animator;
    private NavMeshAgent nav;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        isMove = true;

        curHP = maxHP;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }

    // Update is called once per frame
    void Update()
    {
        if (isMove)
        {
            if (nav.enabled)
            {
                nav.SetDestination(target.position);
                nav.isStopped = !isChase;
            }
            ChasePlayer();
            Targeting();
        }
    }

    private void ChasePlayer()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < chaseDistance && !isChase)
        {
            isChase = true;
            animator.SetBool("isWalk", true);
        }
        else if (distanceToTarget >= chaseDistance && isChase) // 추적 중지
        {
            isChase = false;
            animator.SetBool("isWalk", false);
        }
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
        float targetRadius = 0.5f;
        float targetRange = 5f;

        RaycastHit rayHits;
        bool search = Physics.SphereCast(transform.position
                                            , targetRadius
                                            , transform.forward
                                            , out rayHits
                                            , targetRange
                                            , LayerMask.GetMask("Player"));

        if (search && !isAtk)
        {
            float distance = Vector3.Distance(transform.position
                                              , rayHits.collider.transform.position);
            if (distance < 2)
            {
                Invoke("OnAtk", 0.3f);
            }
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

    private void OnAtk()
    {
        isMove = false;
        isAtk = true;
        animator.SetTrigger("isAtk");

        Invoke("offAtk", 1.5f);
    }

    public void OnChase()
    {
        isMove = true;
    }

    private void offAtk()
    {
        isAtk = false;
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
