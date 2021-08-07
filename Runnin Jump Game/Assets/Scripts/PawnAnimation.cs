using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnAnimation : MonoBehaviour
{
    [SerializeField] private Animator m_Animator;

    public bool Move
    {
        get => m_Animator.GetBool("IsMove");
        set
        {
            m_Animator.SetBool("IsMove", value);
        }
    }

    public bool Jump
    {
        get => m_Animator.GetBool("IsJump");
        set
        {
            m_Animator.SetBool("IsJump", value);
        }
    }

}
