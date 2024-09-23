using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public enum IngredientType
{
    Human,
    Soul,
    Bat
}

[System.Serializable]
public struct CookwareResult
{
    public CookwareType cookwareType;
    public FoodData resultFoodData;
    public float timeToCook;
}*/


[CreateAssetMenu(fileName = "Ingredient Data", menuName = "Scriptable Objects/Ingredient")]
public class IngredientData : ScriptableObject
{
    [Header("Settings")]
    /*[SerializeField]
    private IngredientType type;*/
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    [TextArea]
    private string description;

    /*[Header("Results")]
    [SerializeField]
    private CookwareResult[] cookwareResults;*/

    //public IngredientType Type => type;
    public Sprite Sprite => sprite;
    public string Description => description;

    /*public FoodData GetFoodFromType(CookwareType cookwareType)
    {
        FoodData result = null;
        foreach(CookwareResult cookwareResult in cookwareResults)
        {
            if (cookwareResult.cookwareType == cookwareType)
                result = cookwareResult.resultFoodData;
        }
        return result;
    }

    public float GetTimeFromType(CookwareType cookwareType)
    {
        float result = -1;
        foreach (CookwareResult cookwareResult in cookwareResults)
        {
            if (cookwareResult.cookwareType == cookwareType)
                result = cookwareResult.timeToCook;
        }
        return result;
    }*/
}
