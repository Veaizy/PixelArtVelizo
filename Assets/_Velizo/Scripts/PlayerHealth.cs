using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHealth : MonoBehaviour
{
    public event System.Action OnDeath;

    [SerializeField]
    private string TagHazards = "Finish";

    [Tooltip("Seconds between death detected and restart of the level")]
    [SerializeField]
    private float restartDelay = 1;

    [Header("Don't edit - only shown for easier debuging")]
    [SerializeField]
#pragma warning disable CS0414 // inspector debuging and placeholder for hit points
    private bool isAlive = true;
#pragma warning restore CS0414

    private new Rigidbody2D rigidbody;

    private void Awake()
    {
        rigidbody = this.GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Debug.Log($"Collided with {collision.gameObject.name} {collision.gameObject.tag}");
        if (collision.gameObject.CompareTag(TagHazards))
        {
            Kill();
        }
    }

    public void Kill()
    {
        // perform death
        rigidbody.bodyType = RigidbodyType2D.Static;
        StartCoroutine(delayedRestart(restartDelay));

        // mark as dead
        isAlive = false;
        OnDeath?.Invoke();
    }

    IEnumerator delayedRestart(float seconds = 1)
    {
        yield return new WaitForSeconds(seconds);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
