using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICamrearotation : MonoBehaviour
{
    Transform cam;
    private void Start()
    {
        cam = Camera.main.transform;
    }
    void LateUpdate()
    {
        transform.LookAt(cam.rotation*Vector3.forward+transform.position,cam.rotation*Vector3.up);
    }
}
