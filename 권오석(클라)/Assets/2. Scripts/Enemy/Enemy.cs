using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHP;
    [SerializeField] private int curHP;
    [SerializeField] private Transform target;

    public bool isChase;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private Animator animator;
    private NavMeshAgent nav;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();

        Invoke("ChaseStart", 2);
    }

    // Start is called before the first frame update
    void Start()
    {
        curHP = maxHP;
    }
    private void ChaseStart()
    {
        isChase = true;
        animator.SetBool("isWalk", true);
    }

    // Update is called once per frame
    void Update()
    {
        if(isChase)
            nav.SetDestination(target.position);
    }

    private void FreezeVelocity()
    {
        if (isChase)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
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
