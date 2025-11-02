using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardUpright : MonoBehaviour
{
    void LateUpdate()
    {
        var cam = Camera.main;
        if (!cam) return;

        transform.rotation = Quaternion.LookRotation(cam.transform.forward, Vector3.up);
    }
}
