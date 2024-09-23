using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float tapDuration = 0.015f;
    [SerializeField]
    private float pointNClickCdr = 0.1f;
    [SerializeField]
    private float tapSize = 0.25f;
    [SerializeField]
    private Collider2D moveableArea;
    [SerializeField]
    private LayerMask interactableLayer;
    [SerializeField]
    private LayerMask playerLayer;

    [Header("FootStep SFX")]
    [SerializeField]
    private float footStepCdr = 0.2f;
    public AudioClip[] footStep;

    [Header("Required Components")]
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Selector selector;

    [Header("Debugs")]
    [SerializeField]
    private bool isMoveable;    
    [SerializeField]
    private ControlScheme currentControlScheme;
    [SerializeField]
    private Vector2 movementDirection;
    [SerializeField]
    private Vector2 mousePos;

    private float nextFootStep;
    private float nextPointNClick;
    private float tapStart;
    [SerializeField]
    private Vector2 pointNClickDestination;
    private Vector3 cached_position;

    private bool isTapping;
    private bool newPointNClick;

    private Bounds moveableBounds;

    private Interactable interactableAtDestination;

    private PlayerMovement playerMovement;
    private PlayerInteraction playerInteraction;
    private GameManager gameManager;

    public Vector2 MovementDirection => movementDirection;
    
    public bool IsMoveable
    {
        get { return isMoveable; }
        set
        {
            isMoveable = value;
            if (!isMoveable)
                movementDirection = Vector2.zero;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.main;

        playerMovement = GetComponent<PlayerMovement>();
        playerInteraction = GetComponent<PlayerInteraction>();

        moveableBounds = moveableArea.bounds;
    }

    private void Update()
    {
        if (cached_position != transform.position)
        {
            cached_position = transform.position;
            PlayFootstepSFX();
            UpdateMovingAnimation();
        }
    }

    private void FixedUpdate()
    {
        if (gameManager.IsPaused) return;

        if (currentControlScheme == ControlScheme.DragOnScreen)
        {
            var direction = (mousePos - (Vector2)transform.position);
            if (direction.sqrMagnitude < 0.01f)
            {
                movementDirection = Vector2.zero;
            }
            else if (isTapping)
            {                
                if(Time.time - tapStart > tapDuration)
                    movementDirection = (direction * 100).normalized;
            }
            else
            {
                movementDirection = Vector2.zero;
            }
        }
        else if (currentControlScheme == ControlScheme.PointNClick)
        {
            var direction = (pointNClickDestination - (Vector2)transform.position);
            if (direction.sqrMagnitude < 0.05f)
            {
                movementDirection = Vector2.zero;
                if (newPointNClick)
                {
                    newPointNClick = false;
                    if (interactableAtDestination)
                    {
                        playerInteraction.UseCurrentInteractable(interactableAtDestination);
                        interactableAtDestination = null;
                    }                                          
                }
            }
            else
            {
                movementDirection = (direction * 100).normalized;
            }
        }

        playerMovement.UpdateMovementDirection(movementDirection);
    }

    public void OnTap(InputAction.CallbackContext context)
    {
        if (!isMoveable) return;

        if (currentControlScheme == ControlScheme.DragOnScreen)
        {
            if (context.performed)
            {
                tapStart = Time.time;
                isTapping = true;
            }
            else if (context.canceled)
            {
                if(Time.time - tapStart < tapDuration)
                    playerInteraction.UseCurrentInteractable();
                
                isTapping = false;
            }
        }
        else if(currentControlScheme == ControlScheme.PointNClick)
        {
            if (context.performed)
            {
                newPointNClick = true;

                // Get the initial mouse position
                if (moveableBounds.Contains(mousePos))
                    pointNClickDestination = mousePos;
                else
                    pointNClickDestination = moveableBounds.ClosestPoint(mousePos);

                interactableAtDestination = null;
                var hit = Physics2D.OverlapCircle(pointNClickDestination, tapSize, interactableLayer);
                if (hit) 
                { 
                    if(hit.TryGetComponent(out interactableAtDestination))
                        selector.Select(interactableAtDestination.gameObject);
                    else
                        selector.Show(false);
                }
                else
                {
                    selector.Show(false);
                    hit = Physics2D.OverlapCircle(pointNClickDestination, tapSize, playerLayer);
                    if (hit) playerInteraction.Drop();
                }

                // Offset mouse position for movement
                mousePos = new Vector2(mousePos.x, mousePos.y - 0.5f);
                if (moveableBounds.Contains(mousePos))
                    pointNClickDestination = mousePos;
                else
                    pointNClickDestination = moveableBounds.ClosestPoint(mousePos);
            }
        }
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        if (!isMoveable) return;

        mousePos = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isMoveable) return;

        if (context.performed)
        {
            movementDirection = context.ReadValue<Vector2>().normalized;
        }
        else if (context.canceled)
        {
            movementDirection = Vector2.zero;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!isMoveable) return;

        if (context.performed)
        {
            playerInteraction.UseCurrentInteractable();
        }
    }

    public void OnAlternateInteract(InputAction.CallbackContext context)
    {
        if (!isMoveable) return;

        if (context.performed)
        {
            playerInteraction.AlternateInteraction();
        }
    }

    private void PlayFootstepSFX()
    {
        if (movementDirection.magnitude > 0)
        {
            if (Time.time > nextFootStep)
            {
                ////Play footstep sound
                SoundManager.main.PlayOneShotRandom(footStep);
                nextFootStep = Time.time + footStepCdr;
            }
        }
    }

    private void UpdateMovingAnimation()
    {
        animator.SetFloat("directionX", movementDirection.x);
        animator.SetFloat("directionY", movementDirection.y);

        animator.SetBool("isWalking", movementDirection.magnitude > 0);
        //Debug.Log(movementDirection.magnitude);
    }

    public void SetControlScheme(ControlScheme controlScheme)
    {
        currentControlScheme = controlScheme;
    }

    private void OnDrawGizmos()
    {
        if (currentControlScheme == ControlScheme.PointNClick)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, pointNClickDestination);
            Gizmos.DrawWireSphere(pointNClickDestination, tapSize);
        }
    }
}
