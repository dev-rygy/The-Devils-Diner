using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    Animator myAnimator;

    private void Awake()
    {
        myAnimator = GetComponentInChildren<Animator>();
    }

    public void ToggleMovement(Vector2 movementDirection, bool isMoving)
    {
        if (myAnimator != null)
        {
            myAnimator.SetFloat("moveY", movementDirection.y);
            myAnimator.SetBool("isMoving", isMoving);
        }
    }
}
