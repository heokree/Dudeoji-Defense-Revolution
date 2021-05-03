using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCtrl : MonoBehaviour
{
    private float hp = 100.0f;

    private GameManager gm;

    void OnEnable()
    {
        GameManager.OnGameOver += DestroyBox;
    }

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDisable()
    {
        GameManager.OnGameOver -= DestroyBox;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("SLASH"))
        {
            hp -= 10.0f;
            Debug.Log($"box HP : {hp}");
            // HP바 차감
        
            if (hp <= 0)
            {
                // texture change
                gm.GameOver();
            }
        }
    }

    void DestroyBox()
    {
        // box texture 변경
        
    }
}
