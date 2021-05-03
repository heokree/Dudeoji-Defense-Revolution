using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;


public class PhotonManager : MonoBehaviour
{
    private readonly string gameVersion = "v1.0";
    private string userNickName;

    public TMP_InputField userNickNameText;
    public TMP_InputField roomNameText;
    public TMP_Text userNickNameShow;



#region Unity_Methods
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
#endregion

#region Photon_Methods
    void OnConnectedToMaster()
    {
        Debug.Log("포톤 서버 접속완료.");
    }
#endregion

#region User_Methods
    public void OnStartButtonClicked()
    {
        try
        {
            if (userNickNameText.text != null)
            {
                PhotonNetwork.NickName = userNickNameText.text;
                
                PhotonNetwork.JoinLobby();
            }
            else
            {
                throw new System.Exception("NickName 입력 필요");
            }

        }
        catch(System.Exception e)
        {
            Debug.Log(e);
        }
    }
#endregion

}
