using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject[] items;

    private bool isDoorOpen = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CheckItems())
            {
                OpenDoor();
            }
        }
    }

    private bool CheckItems()
    {
        foreach (GameObject item in items)
        {
            if (!item.activeSelf)
            {
                return false;
            }
        }
        return true;
    }

    private void OpenDoor()
    {
        door.SetActive(false);
        isDoorOpen = true;
    }
}
