using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int Power { get; set; }
    public int Speed { get; set; } = 10;

    private Player p;

    private void Awake()
    {
        p = GameManager.Instance.P;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 dir = p.transform.forward;
        transform.Translate(dir * Time.deltaTime * Speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            Destroy(gameObject);
        }
    }
}
