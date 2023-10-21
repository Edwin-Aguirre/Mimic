using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform mainCameraTransfrom;

    // Start is called before the first frame update
    void Start()
    {
        mainCameraTransfrom = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + mainCameraTransfrom.rotation * Vector3.forward, mainCameraTransfrom.rotation * Vector3.up);
    }
}
