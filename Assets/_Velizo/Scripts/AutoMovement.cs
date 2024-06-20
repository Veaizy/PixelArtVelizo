using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Forward force is added to <seealso cref="bodyToMove"/>, at an impact of <seealso cref="goalVelocity"/> as long as trigger <seealso cref="collider"/> has not been entered.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class AutoMovement : MonoBehaviour
{
    /// <summary> Invoked when the player's path is blocked and they are unable to AutoMove. </summary>
    public event System.Action<bool> OnBlocked;

    [Tooltip("What will be moved forward. Z-Rotation is assumed to be frozen.")]
    [SerializeField]
    private Rigidbody2D bodyToMove;

    [Tooltip("Max speed it will be moved forward")]
    [Range(0,10)]
    [SerializeField]
    private float goalVelocity = 2;

    private new Collider2D collider;

    private bool isBlocked = false;

    private void Awake()
    {
        collider = this.GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (false == isBlocked)
        {
            var horzVelocity = Mathf.Abs(bodyToMove.velocity.x);
            bodyToMove.AddForce(Vector2.right * 10 * (goalVelocity - horzVelocity));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isBlocked = true;
        OnBlocked?.Invoke(isBlocked);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isBlocked = false;
        OnBlocked?.Invoke(isBlocked);
    }
}
