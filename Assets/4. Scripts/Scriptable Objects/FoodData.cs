using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Food Data", menuName = "Scriptable Objects/Food Data")]
public class FoodData : ScriptableObject
{
    [SerializeField]
    private StationType stationType;
    [SerializeField]
    private float timeToCook = 5;
    [SerializeField]
    private Sprite platedSprite; // Ryan made public for Ticket system
    [SerializeField]
    private Sprite unplatedSprite; // Ryan made public for Ticket system
    [SerializeField]
    public IngredientData[] requiredIngredients; // Ryan made public for ticket system
    [SerializeField]
    [TextArea]
    private string description;

    public StationType StationType => stationType;
    public float TimeToCook => timeToCook;
    public Sprite PlatedSprite => platedSprite;
    public Sprite UnplatedSprite => unplatedSprite;
    public string Description => description;

    public bool CheckIngredientRequirements(List<IngredientData> toCookIngredients)
    {
        if(toCookIngredients.Count != requiredIngredients.Length)
        {
            return false;
        }
        else
        {
            bool candidate = true;
            foreach(IngredientData requiredIngredient in requiredIngredients)
            {
                bool found = false;
                foreach(IngredientData toCookIngredient in toCookIngredients)
                {
                    if (requiredIngredient == toCookIngredient)
                        found = true;
                }
                if (!found)
                {
                    candidate = false;
                    break;
                }  
            }

            return candidate;
        }
    }
}
