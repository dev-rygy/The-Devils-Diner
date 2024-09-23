using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveUI : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private Image ingredient1;
    [SerializeField]
    private Image ingredient2;
    [SerializeField]
    private Image ingredientSingle;
    [SerializeField]
    private Button ingredientButton1;
    [SerializeField]
    private Button ingredientButton2;
    [SerializeField]
    private Button ingredientButtonSingle;

    public void Show(bool show)
    {
        panel.SetActive(show);
    }

    public void SetIngredientSingle(Sprite sprite)
    {
        ingredientButton1.gameObject.SetActive(false);
        ingredientButton2.gameObject.SetActive(false);
        ingredientButtonSingle.gameObject.SetActive(true);

        ingredientSingle.sprite = sprite;

    }

    public void SetIngredientDouble(Sprite sprite_1, Sprite sprite_2)
    { 
        ingredientButton1.gameObject.SetActive(true);
        ingredientButton2.gameObject.SetActive(true);
        ingredientButtonSingle.gameObject.SetActive(false);

        ingredient1.sprite = sprite_1;
        ingredient2.sprite = sprite_2;
    }
}
