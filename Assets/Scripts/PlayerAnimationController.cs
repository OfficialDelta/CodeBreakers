using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public void Attack()
    {
        GetComponent<Animator>().SetTrigger("Attack");
    }
    public void OnAnimationEnd()
    {
        GetComponent<Animator>().SetTrigger("Idle");
    }
}
