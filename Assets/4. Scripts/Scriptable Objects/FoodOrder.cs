using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodOrder : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private FoodData foodData;

    public void SetFoodData(FoodData foodData)
    {
        this.foodData = foodData;
        spriteRenderer.sprite = foodData.PlatedSprite;
    }

    public bool Contains(FoodData foodData)
    {
        return this.foodData == foodData;
    }
}
