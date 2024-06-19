using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStageBoundary : MonoBehaviour
{
    private BoxCollider boundary;

    // Start is called before the first frame update
    void Start()
    {
        boundary = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            boundary.enabled = true;
        }
    }
}
