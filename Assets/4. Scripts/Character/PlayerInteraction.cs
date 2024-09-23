using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private static Vector3 DROP_OFFSET = new Vector3(0, 0.5f, 0);

    [Header("Settings")]
    [SerializeField]
    private float scanCdr = 0.01f;
    [SerializeField]
    private float interactionRange = 2f;    // Range for interactions
    [SerializeField]
    private float interactionCdr = 0.5f;    // CoolDownRate for interactions
    [SerializeField]
    private Vector3 interactionOffset;
    [SerializeField]
    private LayerMask interactableLayer;

    [Header("Sfx")]
    [SerializeField]
    private AudioClip[] interactionSounds;

    [Header("Required Components")]
    [SerializeField]
    private Transform itemHolderTransform;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Selector selector;

    [Header("Debugs")]
    [SerializeField]
    private bool showGizmos;
    [SerializeField]
    private Interactable currentInteractable;
    [SerializeField]
    private Dish currentDish;
    [SerializeField]
    private Ingredient currentIngredient;

    public bool interactedWithCookware;

    private float nextScan;
    private float nextInteract;

    private DistanceComparer distanceComparer;
    private Collider2D currentItemCollider;
    private PlayerController playerController;
    private CookReminderUI cookReminder;
    private ControlSchemeManager controlSchemeManager;

    public Dish CurrentDish => currentDish;
    public Ingredient CurrentIngredient => currentIngredient;
    public bool IsHoldingItem => itemHolderTransform.childCount > 0;

    private void Start()
    {
        distanceComparer = new DistanceComparer(transform, interactionOffset);
        playerController = GetComponent<PlayerController>();
        cookReminder = CookReminderUI.main;
        controlSchemeManager = ControlSchemeManager.main;
    }

    private void Update()
    {
        ScanClosetInteractable();
        animator.SetBool("isHolding", IsHoldingItem);
    }

    // Scans for all nearby interactables and sets currentInteractable to the closest (null if no interactables)
    private void ScanClosetInteractable()
    {
        if (Time.time < nextScan) return;
        nextScan = Time.time + scanCdr;

        var hits = Physics2D.OverlapCircleAll(transform.position + interactionOffset, interactionRange, interactableLayer);
        if(hits.Length == 0)
        {
            CookwareTutorial();

            currentInteractable?.Highlight(false, Color.yellow, this);
            currentInteractable = null;

            if (controlSchemeManager.CurrentControlScheme != ControlScheme.PointNClick)
                selector.Show(false);
        }
        else
        {
            Array.Sort(hits, distanceComparer);
            var candidate = hits[0].GetComponent<Interactable>();
            if (currentInteractable != candidate)
            {
                //currentInteractable?.Highlight(false, Color.yellow, this);
                currentInteractable = candidate;
                if(controlSchemeManager.CurrentControlScheme != ControlScheme.PointNClick)
                    selector.Show(false);

                if (candidate is ServingQueue) //Type == InteractableType.ServingQueue
                {
                    if ((currentDish != null && currentDish.CurrentFood != null)
                        || ((ServingQueue)candidate).HasDirtyDish)
                    {
                        //currentInteractable.Highlight(true, Color.yellow, this);
                        if (controlSchemeManager.CurrentControlScheme != ControlScheme.PointNClick)
                            selector.Select(currentInteractable.gameObject);
                    }
                }
                else if (candidate is Ingredient) //.Type == InteractableType.Ingredient
                {
                    if (currentDish == null && currentIngredient == null)
                    {
                        //currentInteractable.Highlight(true, Color.yellow, this);
                        if (controlSchemeManager.CurrentControlScheme != ControlScheme.PointNClick)
                            selector.Select(currentInteractable.gameObject);
                    }
                }
                else
                {
                    //currentInteractable.Highlight(true, Color.yellow, this);
                    if (controlSchemeManager.CurrentControlScheme != ControlScheme.PointNClick)
                        selector.Select(currentInteractable.gameObject);
                }
            }
        } 
    }

    private void CookwareTutorial()
    {
        /*if (interactedWithCookware)
        {
            if (currentInteractable == null || (currentInteractable != null && currentInteractable is not Cookware)) //.Type != InteractableType.CookWare
            {
                interactedWithCookware = false;
                cookReminder.Show(true);
            }
        }*/
    }

    [ContextMenu("Use Current Interactable")]
    public void UseCurrentInteractable(Interactable interactable = null)
    {
        if (Time.time < nextInteract) return;
        nextInteract = Time.time + interactionCdr;

        if (cookReminder.IsShowing)
            cookReminder.Show(false);

        if (controlSchemeManager.CurrentControlScheme == ControlScheme.PointNClick && interactable != currentInteractable)
        {
            Debug.Log($"Current interactable does not match {interactable?.name} =!= {currentInteractable?.name}");
            return;
        }            

        if (currentInteractable)
        {
            Debug.Log("Interact");
            currentInteractable.Interact(this);
            //Play sound here
            SoundManager.main.PlayOneShotRandom(interactionSounds);
            //
            if (controlSchemeManager.CurrentControlScheme == ControlScheme.PointNClick)
                selector.Show(false);
        }
        else
        {
            Debug.Log("Drop");
            // Play Drop sound here
            Drop();
        }  
    }

    public void AlternateInteraction()
    {
        if (Time.time < nextInteract) return;
        nextInteract = Time.time + interactionCdr;

        if (currentInteractable )
        {
            if(currentInteractable is Station)
                ((Station)currentInteractable).QuickRemoveIngredient();
            // Move this somewhere else??
            if (currentInteractable is ServingQueue) 
                ((ServingQueue)currentInteractable).MoveUpFoodOrder();
        }
    }

    public bool Pickup(Transform item)
    {
        // If player is holding an item, drop it
        // Drop();

        if (IsHoldingItem)
        {
           //Debug.Log("Player already hold an item");
           return false;
        }
        else
        {
            if (item.TryGetComponent(out currentDish))
            {
                // picked up a dish
            }
            else if (item.TryGetComponent(out currentIngredient))
            {
                // picked up an ingredient
            }
            else
            {
                Debug.Log("Picked up something weird", item);
            }

            item.transform.parent = itemHolderTransform;
            item.transform.localPosition = Vector3.zero;
            currentItemCollider = item.GetComponent<Collider2D>();
            currentItemCollider.enabled = false;

            return true;
        }
    }

    public void Drop()
    {
        if (itemHolderTransform.childCount > 0)
        {
            currentDish = null;
            currentIngredient = null;

            var temp = itemHolderTransform.GetChild(0);
            temp.transform.parent = null;
            temp.transform.position = transform.position - DROP_OFFSET;
            currentItemCollider.enabled = true;
            currentItemCollider = null;
        }
    }

    public IngredientData ConsumeIngredient()
    {
        if (currentIngredient == null)
        {
            return null;
        }
        else
        {
            var result = currentIngredient.Data;
            Destroy(currentIngredient.gameObject);
            return result;
        }
    } 


    public Dish ConsumeDish()
    {
        if (currentDish == null)
        {
            return null;
        }
        else
        {
            var result = currentDish;
            Destroy(currentDish.gameObject);
            currentDish = null;
            return result;
        }
    }

    public class DistanceComparer : IComparer<Collider2D>
    {
        private Transform target;
        private Vector3 offset;

        public DistanceComparer(Transform target, Vector3 offset)
        {
            this.target = target;
            this.offset = offset;
        }

        public int Compare(Collider2D a, Collider2D b)
        {
            var targetPosition = target.position + offset;
            return Vector3.SqrMagnitude(a.bounds.center - targetPosition).CompareTo(Vector3.SqrMagnitude(b.bounds.center - targetPosition));
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + interactionOffset, interactionRange);
    }
}
