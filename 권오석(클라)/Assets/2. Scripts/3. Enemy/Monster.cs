using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using Sirenix.OdinInspector;

public class Monster : MonoBehaviour
{
    public enum Type { Normal, Elite, Boss };

    [Title("Monster Settings")]
    [EnumToggleButtons] public Type monsterType;
    [TabGroup("Monster Settings", "Settings")] public Transform target;
    [TabGroup("Monster Settings", "Settings")]public BoxCollider meleeArea;

    [TabGroup("Monster Drops", "Drops")] public GameObject ammoBox;
    [TabGroup("Monster Drops", "Drops")] public GameObject healthItem;
    [TabGroup("Monster Drops", "Drops")] public GameObject expItem;
    [TabGroup("Monster Drops", "Drops")] public GameObject heavyGun;
    [TabGroup("Monster Drops", "Drops")] public GameObject dropParent;

    protected float chaseDistance; // 플레이어 감지 범위
    public Define.MonsterData data = new Define.MonsterData();
    protected MonsterAtk monsterAtk;

    protected Rigidbody rb;
    protected Animator animator;
    protected NavMeshAgent nav;
    protected bool toWall; // 벽 충돌확인

    protected bool isChase;
    protected bool isAtk;
    protected bool isMove = true;
    protected bool isDead;

    public static event Action<Monster> OnMonsterDie;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && monsterType != Type.Boss)
        {
            target = player.transform;
        }

        dropParent = GameObject.FindGameObjectWithTag("DropParent");
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
        if (monsterType == Type.Boss)
            return;

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

    private void Targeting()
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
        StopToWall();
    }

    private void StopToWall() // 벽 충돌 확인
    {
        Debug.DrawRay(transform.position, transform.forward * 0.7f, Color.green);
        toWall = Physics.Raycast(transform.position, transform.forward, 0.7f, LayerMask.GetMask("Wall"));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            data.CurHP -= weapon.meleeDamage;
            Debug.Log("Melee : " + data.CurHP);
            Vector3 reactVec = transform.position - other.transform.position;

            if (monsterType != Type.Boss)
                StartCoroutine(OnDamage());
            else if (monsterType == Type.Boss)
                StartCoroutine(BossDamage());
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            data.CurHP -= bullet.damage;
            Debug.Log("Range : " + data.CurHP);
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            if (monsterType != Type.Boss)
                StartCoroutine(OnDamage());
            else if (monsterType == Type.Boss)
                StartCoroutine(BossDamage());

        }
    }

    private void DropAmmoBox()
    {
        Vector3 dropPosition = transform.position;
        dropPosition.y = -0.1f;
        dropPosition.x += 1;

        GameObject ammoBoxInstance = Instantiate(ammoBox, dropPosition, Quaternion.identity);
        SetParent(dropParent, ammoBoxInstance);
    }

    private void DropHealthPotion()
    {
        Vector3 dropPosition = transform.position;
        dropPosition.y = -0.1f;
        dropPosition.x += 1;

        GameObject healthPotionInstance = Instantiate(healthItem, dropPosition, Quaternion.identity);
        SetParent(dropParent, healthPotionInstance);
    }

    private void DropExpItem()
    {
        Vector3 dropPosition = transform.position;
        dropPosition.y = -0.1f;
        dropPosition.z -= 1;

        GameObject expItemInstance = Instantiate(expItem, dropPosition, Quaternion.identity);
        SetParent(dropParent, expItemInstance);
    }

    private void SetParent(GameObject parent, GameObject child)
    {
        if (parent != null)
        {
            child.transform.parent = parent.transform;
        }
    }

    private IEnumerator OnDamage()
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

            if (monsterType == Type.Normal)
            {
                DropExpItem();
                DropAmmoBox();
                Destroy(gameObject, 1.5f);
            }

            if (monsterType == Type.Elite)
            {
                DropExpItem();
                DropHealthPotion();
                Destroy(gameObject, 1.5f);
            }

            if (monsterType == Type.Boss)
                Dead();

            OnMonsterDie?.Invoke(this);
        }
        FindObjectOfType<MonsterUI>().UpdateUI();
    }

    private IEnumerator BossDamage()
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
            animator.SetTrigger("onDead");

            Invoke("Dead", 2f);
        }
    }


    public void Dead()
    {
        UI.Instance.ClearPanel();
    }
}
