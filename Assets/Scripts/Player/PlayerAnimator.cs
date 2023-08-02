using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private Animator animator;
    private Player player;

    void Start() {
        player = gameObject.GetComponentInParent<Player>();
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        animator.SetBool(IS_WALKING, player.IsWalking());
    }
}
