using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;


public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "v1.0";
    private string userNickName;
    public static bool LoadSceneAgain {get; set; } = false;

    public TMP_InputField userNickNameText;
    public TMP_InputField roomNameText;
    public TMP_Text userNickNameShow;
    public GameObject lobby;
    public GameObject start;

    // room List를 관리하기 위해 선언하는 roomDict
    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();
    public GameObject roomPrefab;
    public Transform scrollContent;

#region Unity_Methods
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    void Start()
    {
        lobby.SetActive(false);
        start.SetActive(true);
        
        // if(!LoadSceneAgain)
        // {
        //     lobby.SetActive(false);
        //     start.SetActive(true);
        // }
        // else
        // {
        //     lobby.SetActive(true);
        //     start.SetActive(false);
        // }
    }
#endregion

#region Photon_Methods
    public override void OnConnectedToMaster()
    {
        Debug.Log("포톤 서버 접속완료.");
    }

    public override void OnJoinedLobby()
    {
        start.SetActive(false);
        lobby.SetActive(true);
        userNickNameShow.text = $"NickName: {PhotonNetwork.NickName}";
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 완료");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"room 접속 실패: {returnCode}, {message}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"방 join 완료. 방이름 : {PhotonNetwork.CurrentRoom.Name}");

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Play");
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;

        foreach (var room in roomList)
        {
            // 룸이 삭제된 경우
            if (room.RemovedFromList == true)
            {
                roomDict.TryGetValue(room.Name, out tempRoom);
                Destroy(tempRoom);
                roomDict.Remove(room.Name);
            }
            else
            {
                // 새로 만드는 룸
                if (roomDict.ContainsKey(room.Name) == false)
                {
                    GameObject _room = Instantiate(roomPrefab, scrollContent);
                    _room.GetComponent<RoomData>().RoomInfo = room;
                    roomDict.Add(room.Name, _room);
                }
                else
                {
                    roomDict.TryGetValue(room.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = room;
                }
            }
        }
    }
#endregion


#region User_Methods
    public void OnJoinLobbyButtonClicked()
    {
        try
        {
            if (userNickNameText.text != null)
            {
                PhotonNetwork.JoinLobby();
                PhotonNetwork.NickName = userNickNameText.text;
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

    public void OnMakeRoomClick()
    {
        // 룸 속성 지정
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 4;
        
        try
        {
            if (!string.IsNullOrEmpty(roomNameText.text))
            {
                PhotonNetwork.CreateRoom(roomNameText.text, roomOptions);
            }
            else
            {
                throw new System.Exception("RoomName 입력 필요");
            }

        }
        catch(System.Exception e)
        {
            Debug.Log(e);
        }
    }


#endregion

}
