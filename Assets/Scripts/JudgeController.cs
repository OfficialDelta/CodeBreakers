using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgeController : MonoBehaviour
{
    private void Start()
    {
        Attack();
    }
    public void Attack()
    {
        GetComponent<Animator>().SetTrigger("gavel");
    }
    public void OnAnimationEnd()
    {
        GetComponent<Animator>().SetTrigger("Idle");
    }
}
