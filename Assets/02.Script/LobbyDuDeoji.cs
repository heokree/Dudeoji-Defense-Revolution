using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyDuDeoji : MonoBehaviour
{
    private Animator animator;

    private readonly int hashUnderground    = Animator.StringToHash("Underground");
    private readonly int hashHeadOnly    = Animator.StringToHash("HeadOnly");
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(Action());
    }

    IEnumerator Action()
    {
        while(true)
        {
            yield return new WaitForSeconds(4.0f);
            animator.SetBool(hashUnderground, true);
            animator.SetBool(hashHeadOnly, false);

            yield return new WaitForSeconds(1.0f);
            animator.SetBool(hashUnderground, false);
            animator.SetBool(hashHeadOnly, true);

            yield return new WaitForSeconds(4.0f);
            animator.SetBool(hashUnderground, false);
            animator.SetBool(hashHeadOnly, false);
        }
    }
}
