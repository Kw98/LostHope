using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }
    public Type type;
    public int damage;
    public float atkSpeed;

    public BoxCollider meleeArea;

    public Transform bulletParent;
    public Transform bulletPos;
    public GameObject bullet;

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("MeleeAttack");
            StartCoroutine("MeleeAttack");
        }
        else if (type == Type.Range)
        {
            StartCoroutine("RangeAttack");
        }
    }

    IEnumerator MeleeAttack()
    {
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;
    }
    IEnumerator RangeAttack()
    {
        GameObject b = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        b.transform.SetParent(bulletParent);
        Rigidbody bRb = b.GetComponent<Rigidbody>();
        bRb.velocity = bulletPos.forward * 50;
        yield return null;
    }
}
