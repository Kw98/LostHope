using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;

    private bool sprint;
    private Vector3 moveVec;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        float hMove = Input.GetAxisRaw("Horizontal");
        float vMove = Input.GetAxisRaw("Vertical");
        sprint = Input.GetButton("Sprint");

        moveVec = new Vector3(hMove, 0, vMove).normalized;

        transform.position += moveVec * speed * Time.deltaTime;

        animator.SetBool("Walk-Forward", moveVec != Vector3.zero);
        animator.SetBool("Sprint-Forward", sprint);

        transform.LookAt(transform.position + moveVec);
    }
}
