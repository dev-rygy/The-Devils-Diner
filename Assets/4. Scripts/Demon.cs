using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum DemonType // Added by Ryan
{ 
    Imp,
    Brute,
    Hydra,
    Critic,
    Satan,
    Trash
}

public class Demon : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    public DemonType demonType; // Ryan added
    [SerializeField]
    public Sprite portrait; // Ryan Added
    [SerializeField]
    public int point = 100;
    [SerializeField]
    private float timeTilAngry = 30f;
    [SerializeField]
    private float moveDuration = 1f;
    /*[SerializeField]
    public FoodData currentAppetite; // Ryan Added
    [SerializeField]
    private FoodData[] appetite;*/
    [SerializeField]
    private int oneIngredientDishRequired;
    [SerializeField]
    private int twoIngredientsDishRequired;
    [SerializeField]
    private AnimationHandler animationHandler;

    public float TimeTilAngry => timeTilAngry;

    private void Awake()
    {
        animationHandler = GetComponent<AnimationHandler>();
    }

    public void MoveTo(Vector3 position)
    {
        StopAllCoroutines();
        StartCoroutine(MoveCoroutine(position, moveDuration));
    }

    public void MoveTo(Vector3 position, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(MoveCoroutine(position, duration));
    }

    private IEnumerator MoveCoroutine(Vector3 end, float duration)
    {
        var start = transform.position;
        var direction = (end - start).normalized;

        animationHandler.ToggleMovement(direction, true);

        var t = 0f;
        while (t < duration)
        {
            transform.position = 
                Vector3.Lerp(start, end, t/duration);
            t += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }
        transform.position = end;

        animationHandler.ToggleMovement(direction, false);
    }

    public List<FoodData> GetMealCourse()
    {
        var mealCourse = new List<FoodData>();
        mealCourse.Capacity = oneIngredientDishRequired + twoIngredientsDishRequired;
        for (int i = 0; i < oneIngredientDishRequired; i++)
            mealCourse.Add(RecipeManager.main.GetTodayOneIngDishes());
        for (int i = 0; i < twoIngredientsDishRequired; i++)
            mealCourse.Add(RecipeManager.main.GetTodayTwoIngDishes());

        return mealCourse;
    }
}
