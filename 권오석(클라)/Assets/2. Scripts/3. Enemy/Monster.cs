using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class Monster : MonoBehaviour
{
    public enum Type { A, Boss };
    public Type monsterType;
    public Transform target;
    [SerializeField] private BoxCollider meleeArea;
    
    protected float chaseDistance; // 플레이어 감지 범위
    public Define.MonsterData data = new Define.MonsterData();
    protected MonsterAtk monsterAtk;

    protected Rigidbody rb;
    protected Animator animator;
    protected NavMeshAgent nav;

    protected bool isChase;
    protected bool isAtk;
    protected bool isMove;
    protected bool isDead;

    //private bool alreadyCollided = false; // 히트 시 충돌 플래그 설정

    public static event Action<Monster> OnMonsterDie;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }

    public virtual void Init()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.P == null)
            return;

        if (target == null)
            return;

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

    protected void ChasePlayer()
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

    protected void FreezeVelocity()
    {
        if (isChase)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    protected void Targeting()
    {
        if (!isDead)
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
        Invoke("OnChase", 1.5f);
    }

    private void OnChase()
    {
        isMove = true;
    }

    private void offAtk()
    {
        isAtk = false;
    }

    protected void FixedUpdate()
    {
        FreezeVelocity();
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (alreadyCollided)
            //return;

        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            data.CurHP -= weapon.damage;
            Debug.Log("Melee : " + data.CurHP);
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec));

            //alreadyCollided = true;
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            data.CurHP -= bullet.damage;
            Debug.Log("Range : " + data.CurHP);
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec));

            //alreadyCollided = true;
        }
        //alreadyCollided = false;
    }

    IEnumerator OnDamage(Vector3 reactVec)
    {
        yield return new WaitForSeconds(0.1f);
        if (data.CurHP > 0)
        {
            animator.SetTrigger("GetHit");
            yield return new WaitForSeconds(1f);
        }
        else
        {
            gameObject.layer = 11;
            isDead = true;
            isChase = false;
            nav.enabled = false;
            animator.SetTrigger("onDead");

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rb.AddForce(reactVec * 5, ForceMode.Impulse);

            if (monsterType != Type.Boss)
                Destroy(gameObject, 1.5f);

            OnMonsterDie?.Invoke(this);
        }
        FindObjectOfType<MonsterUI>().UpdateUI();
    }
}
