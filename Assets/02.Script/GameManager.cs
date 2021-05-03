using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
#region Field
    public static GameManager instance  = null;
    
    public List<Transform> spawnPoints  = new List<Transform>();
    public GameObject duDeojiPrefab;

    public List<GameObject> duDeojiPool = new List<GameObject>();
    public int maxPool              = 20;
    public float duDeojiSpawnTime   = 2.0f;

    // EVENT Handler
    public bool isGameOver = false;
    public delegate void GameOverHandler();
    public static event GameOverHandler OnGameOver;

    // UI
    public GameObject exitMenu;

    // Photon
    // private PhotonView photonView;

    // Player Spawn
    public Transform player1Pos;
#endregion

#region Unity_Methods
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != null)
        {
            Destroy(this.gameObject);
        }
        
        GameObject player = PhotonNetwork.Instantiate("Player", player1Pos.position, Quaternion.LookRotation(Vector3.forward * -1, Vector3.up));
        // player.transform.LookAt(GetComponent<Transform>().position);
    }

    void Start()
    {
        // photonView = GetComponent<PhotonView>();
        GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>(spawnPoints);
        StartCoroutine(GetDuDeojiPun());
        exitMenu.SetActive(false);

        // duDeojiPrefab = Resources.Load<GameObject>("DuDeoji_N");
        // CreateDuDeojiPool();
        // StartCoroutine(GetDuDeojiInPool());
    }

    // Update is called once per frame
    void Update()
    {
        // ExitMenu open
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!exitMenu.activeSelf)
            {
                exitMenu.SetActive(true);
            }
            else
            {
                exitMenu.SetActive(false);
            }
        }
    }
#endregion

    #region Photon_Methods
    public override void OnLeftRoom()
    {
        PhotonManager.LoadSceneAgain = true;
        SceneManager.LoadScene("Lobby");
    }

#endregion

    public void GameOver()
    {
        isGameOver = true;
        Debug.Log("gameover");
        // gameover UI 호출

        OnGameOver();
    }

    IEnumerator GetDuDeojiPun()
    {
        while(!isGameOver)
        {
            yield return new WaitForSeconds(duDeojiSpawnTime);

            int idx = Random.Range(1, spawnPoints.Count);
            Quaternion rot = Quaternion.RotateTowards(spawnPoints[idx].rotation, spawnPoints[0].rotation, 10.0f * Time.deltaTime);
            GameObject duDeoji = PhotonNetwork.Instantiate("DuDeoji_N", spawnPoints[idx].position, rot);
            // duDeoji.transform.LookAt(spawnPoints[0]);
            // duDeoji.SetActive(true);
        }
    }

    public void OnExitClick()
    {
        PhotonNetwork.LeaveRoom();
    }

/*
    void CreateDuDeojiPool()
    {
        for (int i = 0 ; i < maxPool ; i++)
        {
            GameObject duDeoji = Instantiate<GameObject>(duDeojiPrefab);
            duDeoji.name = $"DuDeoji_N_{i:00}";
            duDeoji.SetActive(false);

            duDeojiPool.Add(duDeoji);
        }
    }

    IEnumerator GetDuDeojiInPool()
    {
        while(!isGameOver)
        {
            yield return new WaitForSeconds(duDeojiSpawnTime);

            foreach (var duDeoji in duDeojiPool)
            {
                if (duDeoji.activeSelf == false)
                {
                    int idx = Random.Range(1, spawnPoints.Count);

                    duDeoji.transform.position = spawnPoints[idx].position;
                    duDeoji.transform.LookAt(spawnPoints[0]);
                    duDeoji.SetActive(true);
                    break;
                }
            }

        }
    }
*/


}
