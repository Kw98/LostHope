﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class Monster : MonoBehaviour
{
    public enum Type { Normal, Elite, Boss };
    public Type monsterType;
    public Transform target;
    public BoxCollider meleeArea;
    
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

    public GameObject ammoBox;
    public GameObject healthItem;
    public GameObject expItem;
    public Transform dropParent;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && monsterType != Type.Boss)
        {
            target = player.transform;
        }

        GameObject dropParentObject = GameObject.FindGameObjectWithTag("DropParent");
        if (dropParentObject != null)
        {
            dropParent = dropParentObject.transform;
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

    IEnumerator OnDamage()
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
                DropAmmoBox();
                DropExpItem();
                Destroy(gameObject, 1.5f);
            }

            if (monsterType == Type.Elite)
            {
                DropHealthItem();
                DropExpItem();
                Destroy(gameObject, 1.5f);
            }

            if (monsterType == Type.Boss)
                Dead();

            OnMonsterDie?.Invoke(this);
        }
        FindObjectOfType<MonsterUI>().UpdateUI();
    }

    IEnumerator BossDamage()
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

    private void DropAmmoBox()
    {
        if (dropParent != null)
        {
            Instantiate(ammoBox, transform.position, Quaternion.identity, dropParent);
        }
        else
        {
            Instantiate(ammoBox, transform.position, Quaternion.identity);
        }
    }

    private void DropHealthItem()
    {
        if (dropParent != null)
        {
            Instantiate(healthItem, transform.position, Quaternion.identity, dropParent);
        }
        else
        {
            Instantiate(healthItem, transform.position, Quaternion.identity);
        }
    }

    private void DropExpItem()
    {
        if (dropParent != null)
        {
            Instantiate(expItem, transform.position, Quaternion.identity, dropParent);
        }
        else
        {
            Instantiate(expItem, transform.position, Quaternion.identity);
        }
    }

    public void Dead()
    {
        UI.Instance.ClearPanel();
    }
}
