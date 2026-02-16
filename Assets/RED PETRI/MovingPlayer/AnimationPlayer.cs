using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlayer : MonoBehaviour
{
    private Animator animator;

    [Header("External data")]
    public float moveX;        
    public bool isGrounded;    


    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetFloat("Speed", Mathf.Abs(moveX));
        animator.SetBool("isGrounded", isGrounded);
    }

    public void PlayJump()
    {
        if (!isGrounded) return;

        animator.ResetTrigger("IsJump"); 
        animator.SetTrigger("IsJump");
    }


}
