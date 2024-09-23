using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientDispenser : Interactable
{
    [Header("Settings")]
    [SerializeField]
    private IngredientData ingredientData;
    [SerializeField]
    private GameObject ingredientPrefab;
    [SerializeField]
    private SpriteRenderer previewSprite;

    public void SetIngredientData(IngredientData newData)
    {
        ingredientData = newData;
        UpdatePreviewSprite();
    }

    [ContextMenu("Update Preview Sprite")]
    private void UpdatePreviewSprite()
    {
        previewSprite.sprite = ingredientData.Sprite;
    }

    public override void Interact(PlayerInteraction playerInteraction)
    {
        if(playerInteraction.IsHoldingItem) return;

        var ingredientObj = Instantiate(ingredientPrefab, transform.position, Quaternion.identity);
        var ingredient = ingredientObj.GetComponent<Ingredient>();

        ingredient.Data = ingredientData;
        ingredient.Interact(playerInteraction);
    }
}
