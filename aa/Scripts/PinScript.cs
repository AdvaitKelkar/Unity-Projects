using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PinScript : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] Rigidbody2D rb;
    private bool isPinned = false;

    private void FixedUpdate()
    {
        if (!isPinned)
        {
            rb.MovePosition(rb.position + Vector2.up * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Rotator"))
        {
            transform.SetParent(collider.transform);
            ScoreScript.currentScore += 1;
            isPinned = true;
        }
        else if(collider.CompareTag("Pin"))
        {
            FindObjectOfType<EndGame>().StopGame();
        }
    }
}
