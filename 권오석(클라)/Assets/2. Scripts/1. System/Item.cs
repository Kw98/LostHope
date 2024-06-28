using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Weapon, Heart }
    public Type type;
    public int value;

    public float distance = 2f;

    private Transform player;

    private Rigidbody rb;
    private SphereCollider spCollider;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        rb = GetComponent<Rigidbody>();
        spCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.P == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > distance)
        {
            transform.Rotate(Vector3.up * 50 * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            rb.isKinematic = true;
            spCollider.enabled = false;
        }
    }
}

