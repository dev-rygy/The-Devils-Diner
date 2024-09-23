using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    [Range(0.01f,0.3f)]
    private float smoothing = 0.1f;

    [Header("Debugs")]
    [SerializeField]
    private float currentSpeedModifier = 1f;

    private Rigidbody2D rigidBody;
    private Vector2 currentVelocity;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Updates playerMovement for the character
    public void UpdateMovementDirection(Vector2 movementDirection)
    {
        var targetVelocity =   movementDirection * speed * currentSpeedModifier;
        rigidBody.velocity = Vector2.SmoothDamp(rigidBody.velocity, targetVelocity, ref currentVelocity, smoothing);
    }

    public void SetSpeedModifier(float newSpeedModifier)
    {
        currentSpeedModifier = newSpeedModifier;
    }
}
