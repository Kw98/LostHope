using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }
    public Type type;

    [Header("Melee")]
    public int meleeDamage = 2;
    public float atkSpeed = 0.8f;

    [Header("Range")]
    public int rangeDamage = 3;

    public Transform bulletParent;
    public Transform bulletPos;
    public GameObject bullet;

    public int maxAmmo = 10;
    public int currentAmmo;
    public int reserveAmmo = 10;
    public float reloadTime = 2f;
    private bool isReloading = false;

    private Player p;

    // ���� �ʱ�ȭ
    void Start()
    {
        currentAmmo = maxAmmo;
        p = GameManager.Instance.P;
    }

    // ���� ���� ������Ʈ
    void Update()
    {
        if (isReloading)
            return;

        if (type == Type.Range && currentAmmo != 10 && currentAmmo <= 0 || Input.GetButtonDown("Reload"))
        {
            StartCoroutine(Reload());
            return;
        }

        if (type == Type.Range && Input.GetKeyDown(KeyCode.F1))
        {
            AddReserveAmmo(10);
        }
    }

    // ���� ���
    public void Use()
    {
        if (type == Type.Melee)
        {
            StartCoroutine("MeleeAttack");
        }
        else if (type == Type.Range)
        {
            if (currentAmmo > 0)
            {
                StartCoroutine("RangeAttack");
            }
            else
            {
                Debug.Log("Out of ammo!");
            }
        }
    }

    // ���� ���� ó�� �ڷ�ƾ
    private IEnumerator MeleeAttack()
    {
        yield return new WaitForSeconds(0.15f);
        p.meleeArea.enabled = true;

        yield return new WaitForSeconds(0.2f);
        p.meleeArea.enabled = false;
    }

    // ���Ÿ� ���� ó�� �ڷ�ƾ
    private IEnumerator RangeAttack()
    {
        GameObject b = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        b.transform.SetParent(bulletParent);
        Rigidbody bRb = b.GetComponent<Rigidbody>();
        bRb.velocity = bulletPos.up * 30;

        Bullet bulletScript = b.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = rangeDamage;
        }
        currentAmmo--; 
        yield return null;
    }

    // ���� ������ ó��
    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        GameManager.Instance.P.Reload(); // �ִϸ��̼� ó��

        yield return new WaitForSeconds(reloadTime);

        int ammoToReload = maxAmmo - currentAmmo;
        if (reserveAmmo >= ammoToReload)
        {
            currentAmmo += ammoToReload;
            reserveAmmo -= ammoToReload;
        }
        else
        {
            currentAmmo += reserveAmmo;
            reserveAmmo = 0;
        }

        isReloading = false;
        Debug.Log("Reloaded");
    }

    // ���� ź�� �߰�
    public void AddReserveAmmo(int amount)
    {
        reserveAmmo += amount;
        Debug.Log("Added reserve ammo: " + amount + " rounds");
    }

    // ���� ���ݷ� ����
    public void IncreaseMeleeDamage(int amount)
    {
        if (type == Type.Melee)
        {
            meleeDamage += amount;
        }
    }

    // ���Ÿ� ���ݷ� ����
    public void IncreaseRangeDamage(int amount)
    {
        if (type == Type.Range)
        {
            rangeDamage += amount;
        }
    }
}
