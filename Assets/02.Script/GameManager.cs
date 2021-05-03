using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
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
    }
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>(spawnPoints);
        duDeojiPrefab = Resources.Load<GameObject>("DuDeoji_N");
        CreateDuDeojiPool();
        StartCoroutine(GetDuDeojiInPool());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
#endregion

    public void GameOver()
    {
        isGameOver = true;
        Debug.Log("gameover");
        // gameover UI 호출

        OnGameOver();
    }

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
}
