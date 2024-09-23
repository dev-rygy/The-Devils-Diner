using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : Interactable
{
    [Header("Settings")]
    [SerializeField]
    private IngredientData data;

    [Header("Required Components")]
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public IngredientData Data
    {
        get { return data; }
        set
        {
            data = value;
            spriteRenderer.sprite = data? data.Sprite : null;
        }
    }

    public override void Interact(PlayerInteraction playerInteraction)
    {
        playerInteraction.Pickup(transform);
        /*playerInteraction.PickupIngredient(this);
        if(!playerInteraction.hasIngredient)
            Destroy(gameObject);
        // TODO: animation*/
    }
}
