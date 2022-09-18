using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [Header("Dependencies")]
    public Rigidbody2D rb2D;

    [Header("Movement configuration")]
    public float velocity;

    void Update()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

        if (rb2D != null)
            rb2D.velocity = new Vector2(xInput, yInput) * velocity;
    }
}
