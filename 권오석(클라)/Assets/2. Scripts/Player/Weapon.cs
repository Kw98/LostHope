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
    public Transform bulletPos;
    public GameObject bullet;

    private bool canAttack;

    public void Use()
    {
        if (!canAttack)
            return;

        if (type == Type.Melee)
        {
            StopCoroutine("Melee");
            StartCoroutine("Melee");
        }

        if (type == Type.Range)
        {
            StopCoroutine("Range");
            StartCoroutine("Range");
        }
    }

    IEnumerator Melee()
    {
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;
    }
    IEnumerator Range()
    {
        GameObject insBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);

        Rigidbody bulletRigid = insBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 30;

        yield return null;
    }

    public void SetCanAttack(bool canAttack)
    {
        this.canAttack = canAttack;
    }
}
