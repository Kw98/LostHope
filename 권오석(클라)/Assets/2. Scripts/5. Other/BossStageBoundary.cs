using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStageBoundary : MonoBehaviour
{
    [SerializeField] private float minX, maxX, minY, maxY, minZ, maxZ;

    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        transform.position = pos;
    }
}
