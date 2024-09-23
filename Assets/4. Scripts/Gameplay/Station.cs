using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StationType
{
    Grill,
    Cauldron,
    ChoppingBoard,
}

public class Station : Interactable
{
    [Header("Settings")]
    [SerializeField]
    private bool hideIngredientWhenCooking = true;
    [SerializeField]
    private StationType stationType;
    [SerializeField]
    private AudioClip OnStartCookingSfx;
    [SerializeField]
    private ParticleSystem cookingParticle;
    [SerializeField]
    private FoodData burnedFood; 

    [Header("Required Components")]
    [SerializeField]
    private SpriteRenderer doneIndicator;
    [SerializeField]
    private SpriteRenderer cookedDishRenderer;
    [SerializeField]
    private GameObject ingredientPrefab;
    [SerializeField]
    private RemoveUI removeUI;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SpriteRenderer ingredient1Renderer;
    [SerializeField]
    private SpriteRenderer ingredient2Renderer;
    [SerializeField]
    private SpriteRenderer ingredientSRenderer;

    [Header("Debugs")]
    [SerializeField]
    private bool isCooking;
    [SerializeField]
    private bool isCooked;
    [SerializeField]
    private FoodData currentFood;
    [SerializeField]
    private List<FoodData> recipes;
    [SerializeField]
    private List<IngredientData> toCookIngredients = new List<IngredientData>();

    public PlayerInteraction PlayerInteraction 
    {
        get { return playerInteraction; }
        set { playerInteraction = value; }
    }

    public StationType StationType => stationType;

    protected override void Awake()
    {
        base.Awake();
        toCookIngredients = new List<IngredientData>();
        toCookIngredients.Capacity = 2;
    }

    public override void Interact(PlayerInteraction playerInteraction)
    {
        if (isCooking)
        {
            Debug.Log($"{gameObject.name} is cooking, be patient");
        }
        else
        {
            if (isCooked)
            {
                //Debug.Log("Gave food data to player", gameObject);
                if (playerInteraction.CurrentDish == null)
                {
                    Debug.Log("Player has no dish");
                }
                else
                {
                    if (playerInteraction.CurrentDish.IsDirty)
                    {
                        Debug.Log("Dish is dirty");
                    }
                    else if (playerInteraction.CurrentDish.CurrentFood == null)
                    {
                        playerInteraction.CurrentDish.CurrentFood =
                            currentFood;
                        isCooked = false;
                        toCookIngredients.Clear();
                        currentFood = null;

                        cookedDishRenderer.sprite = null;
                        doneIndicator.enabled = false;
                    }
                    else
                    {
                        Debug.Log("Player's dish is full, cannot pick up more food");
                    }
                }
            }
            else
            {
                playerInteraction.interactedWithCookware = true;
                if (playerInteraction.CurrentIngredient != null && toCookIngredients.Count < 2)
                {
                    toCookIngredients.Add(playerInteraction.ConsumeIngredient());
                    var newCount = toCookIngredients.Count;

                    if (newCount == 1)
                    {
                        ingredient1Renderer.sprite = null;
                        ingredient2Renderer.sprite = null;
                        ingredientSRenderer.sprite = toCookIngredients[0].Sprite;

                        removeUI.SetIngredientSingle(toCookIngredients[0].Sprite);
                        removeUI.Show(true);
                    }
                    else if (newCount == 2)
                    {
                        ingredient1Renderer.sprite = toCookIngredients[0].Sprite;
                        ingredient2Renderer.sprite = toCookIngredients[1].Sprite;
                        ingredientSRenderer.sprite = null;

                        removeUI.SetIngredientDouble(toCookIngredients[0].Sprite, toCookIngredients[1].Sprite);
                        removeUI.Show(true);
                    }
                }
                else
                {
                    // The player is not holding an ingredient
                    //  meaning they are trying to activate the cookware
                    playerInteraction.interactedWithCookware = false;
                    if (toCookIngredients.Count > 0)
                    {
                        // If the station contains any ingredients,
                        //  it will find the matching recipe and cook it
                        currentFood = GetMatchingRecipe();
                        StartCoroutine(CookCoroutine(currentFood));
                        removeUI.Show(false);
                    }
                    else
                    {
                        // No ingredient to cook.
                    }
                }
            }
        }
    }

    public void QuickRemoveIngredient()
    {
        RemoveIngredient(toCookIngredients.Count - 1);
    }

    public void RemoveIngredient(int num)
    {
        if (toCookIngredients.Count <= 0 || isCooking || isCooked || !playerInteraction) return;

        if (playerInteraction.IsHoldingItem)
        {
            //Cannot remove item
        }
        else
        {
            var ingredientObj = Instantiate(ingredientPrefab);
            ingredientObj.GetComponent<Ingredient>().Data = toCookIngredients[num];
            playerInteraction.Pickup(ingredientObj.transform);

            toCookIngredients.RemoveAt(num);

            if (toCookIngredients.Count == 1)
            {
                ingredient1Renderer.sprite = null;
                ingredient2Renderer.sprite = null;
                ingredientSRenderer.sprite = toCookIngredients[0].Sprite;
                removeUI.SetIngredientSingle(toCookIngredients[0].Sprite);
            }
            else
            {
                ingredientSRenderer.sprite = null;
                removeUI.Show(false);
            }
        }
    }

    public override void Highlight(bool enable, Color color, PlayerInteraction playerInteraction)
    {
        base.Highlight(enable, color, playerInteraction);
        removeUI.Show(enable && !isCooking && toCookIngredients.Count > 0 && !isCooked);
    }

    private FoodData GetMatchingRecipe()
    {
        var result = burnedFood;
        foreach(FoodData recipe in recipes)
        {
            if (recipe.CheckIngredientRequirements(toCookIngredients))
                result = recipe;
        }
        return result;
    }

    private IEnumerator CookCoroutine(FoodData food)
    {
        //Debug.Log("Cooking started", gameObject);
        isCooking = true;
        if (cookingParticle)
            cookingParticle.Play();
        if (animator)
            animator.SetBool("isCooking", true);
        if (hideIngredientWhenCooking)
        {
            ingredient1Renderer.sprite = null;
            ingredient2Renderer.sprite = null;
            ingredientSRenderer.sprite = null;
        }
        if (OnStartCookingSfx != null)
            SoundManager.main.PlayOneShot(OnStartCookingSfx);


        yield return new WaitForSeconds(food.TimeToCook);
        isCooking = false;
        isCooked = true;

        cookedDishRenderer.sprite = food.UnplatedSprite;

        if (cookingParticle) 
            cookingParticle.Stop();
        if (animator)
            animator.SetBool("isCooking", false);
        if (!hideIngredientWhenCooking)
        {
            ingredient1Renderer.sprite = null;
            ingredient2Renderer.sprite = null;
            ingredientSRenderer.sprite = null;
        }

        doneIndicator.enabled = true;
        //Debug.Log("Cooking done", gameObject);
    }

    public void AddRecipe(FoodData foodData)
    {
        recipes.Add(foodData);
    }
}
