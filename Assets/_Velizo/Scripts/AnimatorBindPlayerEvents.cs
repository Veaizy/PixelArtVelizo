using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Subscribes to events raised by the player that correspond to visuals in the Animator Controller. AKA binds the Animator to PlayerEvents.
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimatorBindPlayerEvents : MonoBehaviour
{
    private static int caroselIndex = 0;

    [SerializeField]
    private PlayerHealth health;

    [SerializeField]
    private AutoMovement autoMovement;

    [SerializeField]
    private List<RuntimeAnimatorController> AnimatorCarosel;

    private Animator animator;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        animator.runtimeAnimatorController = AnimatorCarosel[caroselIndex];
        animator.SetBool("IsMoving", true);
        animator.SetBool("IsDead", false);
    }

    private void OnEnable()
    {
        health.OnDeath += Health_OnDeath;
        autoMovement.OnBlocked += AutoMovement_OnBlocked;
    }

    private void OnDisable()
    {
        health.OnDeath -= Health_OnDeath;
        autoMovement.OnBlocked -= AutoMovement_OnBlocked;
    }

    private void AutoMovement_OnBlocked(bool isBlocked)
    {
        animator.SetBool("IsMoving", false == isBlocked);
    }

    private void Health_OnDeath()
    {
        animator.SetBool("IsDead", true);
        caroselIndex = (caroselIndex + 1) % AnimatorCarosel.Count;
    }
}
