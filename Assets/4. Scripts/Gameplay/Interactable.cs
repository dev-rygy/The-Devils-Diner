using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]
[RequireComponent(typeof(ShaderController))]
public class Interactable : MonoBehaviour
{
    /*[Header("Interactable Settings")]
    [SerializeField]
    private bool showBoundingBox = true;*/

    private new Collider2D collider;
    protected PlayerInteraction playerInteraction;
    protected ShaderController shader;

    protected virtual void Awake()
    {
        shader = GetComponent<ShaderController>();
        collider = GetComponentInChildren<Collider2D>();
    }

    [ContextMenu("Interact")]
    public virtual void Interact(PlayerInteraction playerInteraction)
    {

    }

    public virtual void Highlight(bool enable, Color color, PlayerInteraction playerInteraction)
    {
        //shader.Highlight(enable, color);
        this.playerInteraction = playerInteraction;
    }

    private void OnDrawGizmos()
    {
        if(collider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
        }
    }
}
