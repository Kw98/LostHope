using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;

    private bool isOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        Enemy.OnEnemyDie += EnemyDead;
    }

    private void OnDestroy()
    {
        Enemy.OnEnemyDie -= EnemyDead;
    }

    private void EnemyDead(Enemy enemy)
    {
        bool allDead = true;
        foreach (GameObject enmeyObj in enemies)
        {
            if (enmeyObj != null)
            {
                allDead = false;
                break;
            }
        }
        if (allDead && !isOpen)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        Debug.Log("문이 열렸습니다.");

        Destroy(gameObject);
    }
}
