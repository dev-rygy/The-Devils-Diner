using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager main;

    [Header("Settings")]
    [SerializeField]
    private MenuSelection menuSelection;
    [SerializeField]
    private Station[] stations;
    [SerializeField]
    private IngredientDispenser[] ingredientDispensers;

    [Header("Debugs")]
    [SerializeField]
    private bool isInitialized = false;
    [SerializeField]
    private List<IngredientData> todayIngredients = new List<IngredientData>();
    [SerializeField]
    private List<FoodData> todayOneIngDishes = new List<FoodData>();
    [SerializeField]
    private List<FoodData> todayTwoIngDishes = new List<FoodData>();

    public bool IsInitialized => isInitialized;

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        menuSelection.GetTodayMenu(3,out todayIngredients, out todayOneIngDishes, out todayTwoIngDishes);

        for (int i = 0; i < todayIngredients.Count; i++)
        {
            ingredientDispensers[i].SetIngredientData(todayIngredients[i]);
        }

        foreach (var station in stations)
        {
            foreach (var dish in todayOneIngDishes)
            {
                if (dish.StationType == station.StationType)
                    station.AddRecipe(dish);
            }

            foreach (var dish in todayTwoIngDishes)
            {
                if (dish.StationType == station.StationType)
                    station.AddRecipe(dish);
            }
        }

        isInitialized = true;
    }

    public FoodData GetTodayOneIngDishes()
    {
        return todayOneIngDishes.GetRandomElement();
    }

    public FoodData GetTodayTwoIngDishes()
    {
        return todayTwoIngDishes.GetRandomElement();
    }
}
