using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerCtrl : MonoBehaviour, IPunObservable
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
    private float rotateH;
    private float rotateV;

    // Photon
    private PhotonView  photonView;
    private Vector3     receivePos  = Vector3.zero;
    private Quaternion  receiveRot  = Quaternion.identity; 
#endregion

#region raycast
    private Ray ray;
    private RaycastHit hit;
    private new Camera camera;
#endregion

    private Transform playerTr;
    // private Transform playerPosTr;
    public Transform hammerTr;
    public Transform hammerTurnSupport;

#endregion

#region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        playerTr = GetComponent<Transform>();
        // playerPosTr = GetComponentInParent<Transform>();
        if (photonView.IsMine)
        {
            camera = Camera.main;
            camera.GetComponent<FallowCam>().TargetTr = playerTr;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            MovePlayer();

            ray = camera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 1000.0f, Color.green);
            
            if(Input.GetMouseButtonDown(0))
            {
                photonView.RPC("ThrowHM_RPC", RpcTarget.AllViaServer, photonView.Owner.NickName);
            }
        }
        else
        {
            DeadReckoning();
        }
    }
#endregion

    void MovePlayer()
    {
        moveH = Input.GetAxis("Horizontal");
        moveV = Input.GetAxis("Vertical");
        rotateH = Input.GetAxis("Mouse X");
        // rotateV = Input.GetAxis("Mouse Y");

        Vector3 moveDir = (Vector3.forward * moveV) + (Vector3.right * moveH);
        playerTr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed);
        playerTr.Rotate((Vector3.up * rotateH) * Time.deltaTime * rotateSpeed);
        // playerTr.Rotate(((Vector3.up * rotateH) + (Vector3.right * -1 * rotateV)) * Time.deltaTime * rotateSpeed);
    }

    void DeadReckoning()
    {
        if ((playerTr.position - receivePos).sqrMagnitude > 3.0f * 3.0f)
        {
            playerTr.position = receivePos;
        }
        else
        {
            playerTr.position = Vector3.Lerp(playerTr.position, receivePos, Time.deltaTime * 10.0f);
        }
        playerTr.rotation = Quaternion.Slerp(playerTr.rotation, receiveRot, Time.deltaTime * 10.0f);
    }

    [PunRPC]
    void ThrowHM_RPC(string playerName)
    {
        Debug.Log($"ThrowHM_RPC playerName: {playerName}");
        StartCoroutine(ThrowHM());
    }

    IEnumerator ThrowHM()
    {
        Vector3 originalPos = playerTr.position - hammerTr.position;

        if(Physics.Raycast(ray, out hit))
        {
            hammerTr.RotateAround(hammerTurnSupport.position, hammerTr.right, 90.0f);
            hammerTr.position = Vector3.Lerp(hammerTr.position, hit.point, 10.0f);
            
            yield return new WaitForSeconds(0.1f);

            hammerTr.RotateAround(hammerTurnSupport.position, hammerTr.right, -90.0f);
            hammerTr.position = Vector3.Lerp(hammerTr.position, (playerTr.position - originalPos), 10.0f);
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(playerTr.position);
            stream.SendNext(playerTr.rotation);
        }
        else
        {
            receivePos = (Vector3) stream.ReceiveNext();
            receiveRot = (Quaternion) stream.ReceiveNext();
        }
    }
}
