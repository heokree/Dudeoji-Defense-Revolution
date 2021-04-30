using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
#region Fields
#region move
    [Header("Move/Rotate Speed")]
    [Range(3.0f, 30.0f)]
    public float moveSpeed = 10.0f;
    [Range(30.0f, 300.0f)]
    public float rotateSpeed = 100.0f;

    private float moveH;
    private float moveV;
    private float rotate;
#endregion

#region raycast
    private Ray ray;
    private RaycastHit hit;
    private new Camera camera;
#endregion

    private Transform playerTr;
    public Transform hammerTr;
    public Transform hammerTurnSupport;

#endregion

#region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        playerTr = GetComponent<Transform>();
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        ray = camera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 1000.0f, Color.green);

        MovePlayer();

        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("click");
            StartCoroutine(Smash());
        }
    }
#endregion

    void MovePlayer()
    {
        moveH = Input.GetAxis("Horizontal");
        moveV = Input.GetAxis("Vertical");
        rotate = Input.GetAxis("Mouse X");

        Vector3 moveDir = (Vector3.forward * moveV) + (Vector3.right * moveH);
        playerTr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed);
        playerTr.Rotate(Vector3.up * Time.deltaTime * rotate * rotateSpeed);
    }

    IEnumerator Smash()
    {
        Vector3 originalPos = hammerTr.position;
        Debug.Log($"Step1: originalPos: {originalPos} hammerTr.position : {hammerTr.position}");

        if(Physics.Raycast(ray, out hit))
        {
            Debug.Log("hit");

            hammerTr.RotateAround(hammerTurnSupport.position, hammerTr.right, 90.0f);
            hammerTr.position = Vector3.Lerp(hammerTr.position, hit.point, 10.0f);
            
            Debug.Log($"Step2: originalPos: {originalPos} hammerTr.position : {hammerTr.position}");

            yield return new WaitForSeconds(1.0f);

            hammerTr.RotateAround(hammerTurnSupport.position, hammerTr.right, -90.0f);
            hammerTr.position = Vector3.Lerp(originalPos, hammerTr.position, 10.0f);
            
            Debug.Log($"Step3: originalPos: {originalPos} hammerTr.position : {hammerTr.position}");
        }

    }
}
