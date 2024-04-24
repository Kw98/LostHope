using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Player p;

    protected Define.MonsterData data = new Define.MonsterData();

    private MonsterState state = MonsterState.Run;

    private float hitTimer;
    protected float moveDistance = 0f;
    private float atkTimer;
    public virtual void Init()
    {
        gameObject.tag = "monster";
        //state = MonsterState.Run;
        //GetComponent<Collider2D>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Define.state != GameState.Play)
            return;

        if (GameManager.Instance.P == null || state == MonsterState.Dead)
            return;

        if (p == null)
            p = GameManager.Instance.P;

        if (state == MonsterState.Hit)
        {
            hitTimer -= Time.deltaTime;
            if (hitTimer <= 0)
            {
                //state = MonsterState.Run;
            }
            else
                return;
        }

        float mDis = Vector2.Distance(p.transform.position, transform.position);
        if (mDis > data.AtkDistance)
        {
            Vector2 dis = p.transform.position - transform.position;
            Vector3 dir = dis.normalized * Time.deltaTime * data.Speed;

            transform.Translate(dir);
        }
        else
        {
            atkTimer += Time.smoothDeltaTime;

            if (atkTimer >= data.AtkDelay)
            {
                atkTimer = 0;
                p.SetCurHP -= data.Power;
            }
        }
    }

    public void Hit(int damage)
    {
        if (data.CurHP <= 0)
            return;

        data.CurHP -= damage;
        hitTimer = data.HitDelayTime;
        //state = MonsterState.Hit;

        if (data.CurHP <= 0)
        {
            //state = MonsterState.Dead;
            gameObject.tag = "Untagged";
            //GetComponent<Collider2D>().enabled = false;
        }
    }

    public void Dead()
    {
        //Exp e = Instantiate(exp, transform.position, Quaternion.identity);
        //e.SetTarget(p);
        //e.EXP = data.Exp;
    }

    //public void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Bullet b = collision.GetComponent<Bullet>();

    //    if (b != null)
    //    {
    //        Hit(b.Power);
    //        Debug.Log($"Bullet : {b.Power}");
    //        Destroy(collision.gameObject);
    //    }
    //}
}
