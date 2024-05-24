using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private GameObject[] monsters;

    private bool isOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        Monster.OnMonsterDie += MonsterDead;
    }

    private void OnDestroy()
    {
        Monster.OnMonsterDie -= MonsterDead;
    }

    private void MonsterDead(Monster monster)
    {
        bool allDead = true;
        for (int i = 0; i < monsters.Length; i++)
        {
            if (monsters[i] == monster.gameObject)
            {
                monsters[i] = null;
            }

            if (monsters[i] != null)
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
