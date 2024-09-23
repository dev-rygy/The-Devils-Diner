using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct IngredientToDishEntry
{
    public IngredientData ingredientData;
    public FoodData foodData;
}

[CreateAssetMenu(fileName = "MenuSelection", menuName = "Scriptable Objects/Menu Selection")]
public class MenuSelection : ScriptableObject
{   
    [SerializeField]
    private IngredientToDishEntry[] ingredientToDishes;

    [SerializeField]
    private FoodData[] twoIngredientsDishes;

    [ContextMenu("Get Menu Test")]
    public void GetMenuTest()
    {
        GetTodayMenu(3,out List<IngredientData> todayIngredients, out List<FoodData> todayOneIngDishes, out List<FoodData> todayTwoIngDishes);
        string output = "";
        foreach (var dish in twoIngredientsDishes)
        {
            output = output + dish.name + ":" + dish.StationType + ", ";
        }

        output.Substring(0, output.Length - 1);

        output += "\n";
        foreach (var dish in todayOneIngDishes)
        {
            output = output + dish.name + ", ";
        }

        Debug.Log(output.Substring(0, output.Length - 1));
    }

    public void GetTodayMenu(int numOfTwoIngDishes,out List<IngredientData> todayIngredients,out List<FoodData> todayOneIngDishes, out List<FoodData> todayTwoIngDishes)
    {
        todayIngredients = new List<IngredientData>();
        todayOneIngDishes = new List<FoodData>();
        todayTwoIngDishes = new List<FoodData>();

        HashSet<StationType> selectedTypes = new HashSet<StationType>();
        HashSet<int> selectedIndices = new HashSet<int>();

        var length = twoIngredientsDishes.Length;
        var count = 0;

        while (todayTwoIngDishes.Count < numOfTwoIngDishes)
        {
            count++;
            if (count > 999)
            {
                Debug.LogError("Menu selections take too long");
                return;
            } 
                
            int randomIndex = UnityEngine.Random.Range(0, length);

            // Make sure not to select first and last item simultaneously
            if ((randomIndex == 0 && selectedIndices.Contains(length - 1)) ||
                (randomIndex == length - 1 && selectedIndices.Contains(0)))
                continue;

            // Make sure not to select next to each other
            if (selectedIndices.Contains(randomIndex - 1) || selectedIndices.Contains(randomIndex + 1))
                continue;

            FoodData candidateFood = twoIngredientsDishes[randomIndex];

            // Make sure not to select the same StationType
            if (selectedTypes.Contains(candidateFood.StationType))
                continue;

            // If all conditions are met, add to selected
            todayTwoIngDishes.Add(candidateFood);
            selectedTypes.Add(candidateFood.StationType);
            selectedIndices.Add(randomIndex);
        }

        Dictionary<IngredientData, FoodData> conversionDictionary = new Dictionary<IngredientData, FoodData>();
        foreach (var conversion in ingredientToDishes) 
            conversionDictionary[conversion.ingredientData] = conversion.foodData;

        foreach (var dish in todayTwoIngDishes)
        {
            todayIngredients.Add(dish.requiredIngredients[0]);
            todayIngredients.Add(dish.requiredIngredients[1]);
            todayOneIngDishes.Add(conversionDictionary[dish.requiredIngredients[0]]);
            todayOneIngDishes.Add(conversionDictionary[dish.requiredIngredients[1]]);
        }
    }
}
