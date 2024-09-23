using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dish : Interactable
{
    [Header("Settings")]
    [SerializeField]
    private FoodData currentFood;
    [SerializeField]
    private Sprite dirtySprite;
    [SerializeField]
    private bool isDirty;

    [Header("RequiredComponents")]
    [SerializeField]
    private SpriteRenderer foodRenderer;

    public bool IsDirty => isDirty;

    public FoodData CurrentFood
    {
        get { return currentFood; }
        set
        {
            currentFood = value;
            foodRenderer.sprite = currentFood ? currentFood.PlatedSprite : null;
        }
    }

    public override void Interact(PlayerInteraction playerInteraction)
    {
        playerInteraction.Pickup(transform);
    }

    [ContextMenu("Make Dirty")]
    public void MakeDirty()
    {
        if (currentFood == null) return;

        CurrentFood = null;
        isDirty = true;
        foodRenderer.sprite = dirtySprite;
    }
}
