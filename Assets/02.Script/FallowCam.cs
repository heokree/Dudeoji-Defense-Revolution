using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallowCam : MonoBehaviour
{
    public Transform TargetTr {get; set;}

    public float distance   = 5.0f;
    public float height     = 5.0f;
    public float damping    = 10.0f;

    private Transform cameraTr;

    // Start is called before the first frame update
    void Start()
    {
        cameraTr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 cameraPosition = TargetTr.position + (TargetTr.forward * -1 * distance) + (TargetTr.up * height);

        cameraTr.position = Vector3.Lerp(cameraTr.position, cameraPosition, Time.deltaTime * damping);
        cameraTr.LookAt(TargetTr.position);
    }
}
