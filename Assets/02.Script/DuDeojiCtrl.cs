using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DuDeojiCtrl : MonoBehaviour
{
#region Fields
    private Transform duDeojiTr;
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    public enum STATE {UNDERGROND, IDLE, RUN, ATTACK, DIE}
    private STATE state = STATE.UNDERGROND;
    private bool isDie {get; set;}
    private bool lookAtBox = false;

    private readonly int hashUnderground    = Animator.StringToHash("Underground");
    private readonly int hashRun            = Animator.StringToHash("Run");
    private readonly int hashAttack         = Animator.StringToHash("Attack");
    private readonly int hashDie            = Animator.StringToHash("Die");

    public float attackDistance     = 2.0f;
    public float moveSpeed          = 5.0f;
    public float idleTime           = 2.0f;

    public Transform boxTr;
    public Transform startRunPoint;
    public GameObject hole;
#endregion

#region Unity_Methods
    void Awake ()
    {
        duDeojiTr       = GetComponent<Transform>();
        navMeshAgent    = GetComponent<NavMeshAgent>();
        animator        = GetComponent<Animator>();
        isDie           = false;
    }

    void OnEnable()
    {
        boxTr           = GameObject.FindGameObjectWithTag("BOX").GetComponent<Transform>();
        startRunPoint   = GameObject.FindGameObjectWithTag("SRP").GetComponent<Transform>();
        hole            = duDeojiTr.Find("Hole").gameObject;

        GameManager.OnGameOver += this.DudeojiWin;

        StartCoroutine(CheckState());
        StartCoroutine(Action());
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDisable()
    {
        GameManager.OnGameOver -= this.DudeojiWin;
    }
#endregion

#region Event
    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("HAMMER"))
        {
            OnHit();
        }
    }
#endregion

#region User_Methods
    IEnumerator CheckState()
    {
        while(!isDie)
        {
            if (state == STATE.DIE)
            {
                yield break;
            }

            // BOX 와 두더지 사이 거리 계산
            float distance = Vector3.Distance(duDeojiTr.position, boxTr.position);

            if (distance <= attackDistance)
            {
                state = STATE.ATTACK;
                yield return new WaitForSeconds(0.3f);
            }
            else if (distance <= Vector3.Distance(startRunPoint.position, boxTr.position))
            {
                state = STATE.RUN;
                yield return new WaitForSeconds(0.2f);
            }
            else if (state == STATE.IDLE)
            {
                yield return new WaitForSeconds(idleTime);
                state = STATE.UNDERGROND;
            }
            else if (state == STATE.UNDERGROND)
            {
                float randomGoUp = Random.Range(1.0f, 1.5f);
                yield return new WaitForSeconds(randomGoUp);
                state = STATE.IDLE;
            }
            
        }
    }

    IEnumerator Action()
    {
        while (!isDie)
        {
            switch (state)
            {
                case STATE.UNDERGROND:
                    navMeshAgent.SetDestination(boxTr.position);
                    navMeshAgent.isStopped = false;
                    animator.SetBool(hashUnderground, true);
                    hole.SetActive(false);
                    GetComponent<CapsuleCollider>().enabled = false;
                    break;

                case STATE.IDLE:
                    navMeshAgent.isStopped = true;
                    animator.SetBool(hashUnderground, false);
                    hole.SetActive(true);
                    GetComponent<CapsuleCollider>().enabled = true;
                    break;

                case STATE.RUN:
                    navMeshAgent.isStopped = false;
                    animator.SetTrigger(hashRun);
                    break;

                case STATE.ATTACK:
                    if(!lookAtBox)
                    {
                        duDeojiTr.LookAt(boxTr.position);
                    }
                    animator.SetTrigger(hashAttack);
                    yield return new WaitForSeconds(1.0f);
                    break;

                case STATE.DIE:
                    animator.SetTrigger(hashDie);
                    GetComponent<CapsuleCollider>().enabled = false;
                    navMeshAgent.isStopped = true;
                    isDie = true;

                    Invoke("ReturnPool", 1.0f);
                    break;
            }
            yield return new WaitForSeconds(0.2f);
        }

    }


    void OnHit()
    {
        // 맞은 effect

        // GameManager Score
        animator.SetTrigger(hashDie);
        state = STATE.DIE;
    }

    void ReturnPool()
    {
        GetComponent<CapsuleCollider>().enabled = true;
        navMeshAgent.isStopped = false;
        isDie = false;
        state = STATE.UNDERGROND;
        
        this.gameObject.SetActive(false);
    }

    public void DudeojiWin()
    {
        StopAllCoroutines();
        navMeshAgent.isStopped = true;

        // animator.SetTrigger("");
    }
#endregion
}
