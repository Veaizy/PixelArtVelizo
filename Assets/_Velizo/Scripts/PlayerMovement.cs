using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Tooltip("RigidBody affected by input from this component.")]
    [SerializeField]
    private new Rigidbody2D rigidbody;

    [Range(0, 100)]
    [SerializeField]
    [Tooltip("The initial force of a jump")]
    private float jumpStart = 16;

    [Header("Don't edit - only shown for easier debuging")] //
    [SerializeField]
    [Tooltip("The force of the jump as the jump button is held down")]
    private float jumpRemaining;

    [SerializeField]
    private bool isJumpPressed;

    private void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    /// <summary> Called via Send Message from PlayerInput component </summary>
    private void OnJump(InputValue value)
    {
        isJumpPressed = value.isPressed;
        if (isJumpPressed) // verify it's valid to jump from this position
        {
            // override if not on ground. detected by how quickly player is moving up or down
            // NOTE: it's possible for a player to double jump if they hit jump at the pinicle of their jump. At the margin 0.1, this would take excessive skill, so leaving it in.
            isJumpPressed = Mathf.Abs(rigidbody.velocity.y) < 0.1;

            // jumpRemaining only needs to be reset when isJumpPressed, but there's no harm in also setting it when isJumpPressed == false
            jumpRemaining = jumpStart;
        }
    }

    private void FixedUpdate()
    {
        if (isJumpPressed && jumpRemaining > float.Epsilon)
        {
            jumpRemaining /= 32; // how quickly the jumpStart degrades while jump held
            rigidbody.AddForce(Vector2.up * jumpStart);
        }
    }
}
