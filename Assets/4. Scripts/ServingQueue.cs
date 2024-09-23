using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FoodOrderTicket
{
    public FoodData foodData;
    public FoodOrder foodOrder;
    public Ticket ticket;

    public FoodOrderTicket(FoodData foodData, FoodOrder foodOrder, Ticket ticket)
    {
        this.foodData = foodData;
        this.foodOrder = foodOrder;
        this.ticket = ticket;
    }
}

public static class FoodOrderTicketListExtentions
{
    public static bool ContainFood(this List<FoodOrderTicket> list, FoodData foodData, out int index)
    {
        var count = list.Count;
        for(int i = 0; i < count; i++)
        {
            if (list[i].foodData == foodData)
            {
                index = i;
                return true;
            }  
        }

        index = -1;
        return false;
    }
}

public class ServingQueue : Interactable
{
    [Serializable]
    private class DemonQueuePoint
    {
        public Demon demon;
        public Transform point;

        public DemonQueuePoint(Demon demon, Transform point)
        {
            this.demon = demon;
            this.point = point;
        }
    }

    [Header("Settings")]
    [SerializeField]
    private int queueNumber;
    [SerializeField]
    private int maxDemonInQueue = 3;
    [SerializeField]
    private Sprite rugSprite;
    [SerializeField]
    private float newOrderOffet = 0.25f;
    [SerializeField]
    private float sideWalkDuration = 1f;
    [SerializeField]
    private float offScreenDuration = 3.5f;

    [Header("Required Components")]
    [SerializeField]
    private GameObject dirtyDishPrefab;
    [SerializeField]
    private Transform dirtyDishSpawnPoint;
    [SerializeField]
    private GameObject foodOrderPrefab;
    [SerializeField]
    private Transform foodOrdersDisplay;
    [SerializeField]
    private Transform sideWalkDestination;
    [SerializeField]
    private Transform offScreenDestination;
    [SerializeField]
    private Transform demonsTransform;

    [Header("Debugs")]
    [SerializeField]
    private int demonCount = 0;
    [SerializeField]
    private List<FoodOrderTicket> currentMealCourse;
    [SerializeField]
    private DemonQueuePoint[] demonQueuePoints;

    private float nextDemonAngry;

    private Demon demonInService;    
    private Collider2D interactionCollider;
    private GameObject dirtyDish;   

    private GameManager gameManager;
    private PoolManager poolManager;
    private TicketUI ticketUI;

    public bool IsFull => demonCount >= maxDemonInQueue;
    public bool HasDirtyDish => dirtyDish != null;
    public int DemonCount => demonCount;

    private void Start()
    {
        gameManager = GameManager.main;
        poolManager = PoolManager.main;
        ticketUI = TicketUI.main;
        interactionCollider = GetComponent<Collider2D>();
        currentMealCourse = new List<FoodOrderTicket>();
    }

    private void Update()
    {
        if (!Application.isPlaying) return;

        if(dirtyDishSpawnPoint.childCount > 0)
        {
            foodOrdersDisplay.gameObject.SetActive(false);
            interactionCollider.enabled = true;
        }
        else
        {
            if (demonInService != null)
            {
                foodOrdersDisplay.gameObject.SetActive(true);
                interactionCollider.enabled = true;
            }
            else
            {
                foodOrdersDisplay.gameObject.SetActive(false);
                interactionCollider.enabled = false;
            }
        }

        if (Time.time > nextDemonAngry && nextDemonAngry > 0 && demonInService != null)
        {
            nextDemonAngry = -1;
            RemoveDemon();
            gameManager.LoseStar();
        }
    }

    public override void Interact(PlayerInteraction playerInteraction)
    {
        
        if (dirtyDish != null)
        {
            if (playerInteraction.Pickup(dirtyDish.transform))
                dirtyDish = null;
        }
        else if (playerInteraction.CurrentDish)
        {
            if(currentMealCourse.ContainFood(playerInteraction.CurrentDish.CurrentFood, out int index))
            {
                RemoveCurrentMealAt(index);

                playerInteraction.ConsumeDish();
                dirtyDish = Instantiate(dirtyDishPrefab, dirtyDishSpawnPoint);

                if (currentMealCourse.Count == 0)
                {
                    gameManager.GainPoint(demonInService.point);
                    HighScoreUI.main.UpdateValue(gameManager.CurrentScore);
                    if (demonInService.demonType == DemonType.Critic)
                        gameManager.GainStar();

                    RemoveDemon();
                }
            }
            
        }
    }

    private void RemoveCurrentMealAt(int index)
    {
        ticketUI.RemoveTicket(currentMealCourse[index].ticket);
        RemoveFoodOrder(currentMealCourse[index].foodOrder);
        currentMealCourse.RemoveAt(index);
    }

    private void RemoveCurrentMealAll()
    {
        for(int i = 0; i < currentMealCourse.Count; i++)
        {
            ticketUI.RemoveTicket(currentMealCourse[i].ticket);
            RemoveFoodOrder(currentMealCourse[i].foodOrder);
        }

        currentMealCourse.Clear();
    }

    public void AddDemon(GameObject demonPrefab)
    {
        if (IsFull) return;
        demonCount++;

        var index = GetLastEmptyQueueIndex();
        //Debug.Log($"index = {index}");

        var demonObj = poolManager.Spawn(demonPrefab,
            demonQueuePoints[demonQueuePoints.Length - 1].point.position, 
            Quaternion.identity, demonsTransform);
        var demonSct = demonObj.GetComponent<Demon>();
        demonSct.MoveTo(demonQueuePoints[index].point.position);
        demonQueuePoints[index].demon = demonSct;

        if (index == 0)
            SetDemonInService(demonSct);
    }

    private int GetLastEmptyQueueIndex()
    {
        for(int i  = demonQueuePoints.Length - 1; i >= 0; i--)
        {
            if (demonQueuePoints[i].demon != null)
            {
                return i + 1;
            }
        }

        if (demonQueuePoints[0].demon == null)
            return 0;
        else
            return -1;
    }

    [ContextMenu("RemoveDemon")]
    public void RemoveDemon()
    {
        if (demonCount == 0) return;
        demonCount--;

        // Notify the GameManager of the removing demon
        var demonToRemove = demonQueuePoints[0].demon;
        gameManager.NotifyRemoveDemon(this, demonToRemove.demonType);

        // Remove the demon
        StartCoroutine(RemoveDemonCoroutine(demonToRemove));

        RemoveCurrentMealAll();
        SetDemonInService(null);

        // Move all demon spot up 1 position, except for the last spot
        for(int i = 0; i < demonQueuePoints.Length - 1; i++)
            demonQueuePoints[i].demon = demonQueuePoints[i + 1].demon;

        // Set the last demon spot to null
        demonQueuePoints[demonQueuePoints.Length - 1].demon = null;

        // Move the demon to their correct spot
        for(int i = 0; i < demonQueuePoints.Length; i++)
            demonQueuePoints[i].demon?.MoveTo(demonQueuePoints[i].point.position);

        // Serve the demon at the beginning of the queue
        SetDemonInService(demonQueuePoints[0].demon);
    }

    private IEnumerator RemoveDemonCoroutine(Demon demon)
    {
        demon.MoveTo(sideWalkDestination.position, 1f);
        yield return new WaitForSeconds(sideWalkDuration);
        demon.MoveTo(offScreenDestination.position, 3f);
        yield return new WaitForSeconds(offScreenDuration);
        poolManager.Despawn(demon.gameObject);
        gameManager.NotifyDemonDespawn();
    }

    private void SetDemonInService(Demon demon)
    {
        demonInService = demon;
        if(demon == null)
        {
            currentMealCourse.Clear();
            foodOrdersDisplay.gameObject.SetActive(false);
        }
        else
        {
            nextDemonAngry = Time.time + demon.TimeTilAngry / gameManager.TimeScale;

            // Get the demon meal course
            var demonMealCourse = demon.GetMealCourse();

            // Add the meal to ticketUI
            for(int i = 0; i < demonMealCourse.Count; i++)
            {
                var foodData = demonMealCourse[i];
                currentMealCourse.Add(new FoodOrderTicket(foodData,
                    AddFoodOrder(foodData),
                    ticketUI.AddTicket(demon, foodData, queueNumber, rugSprite)));
            }

            foodOrdersDisplay.gameObject.SetActive(true);
        }
    }

    private FoodOrder AddFoodOrder(FoodData foodData)
    {
        var foodOrderObj = Instantiate(foodOrderPrefab, foodOrdersDisplay);
        foodOrderObj.transform.SetAsLastSibling();
        var foodOrderSct = foodOrderObj.GetComponent<FoodOrder>();
        foodOrderSct.SetFoodData(foodData);
        ReorganizeFoodOrder();

        return foodOrderSct;
    }

    private void RemoveFoodOrder(FoodOrder foodOrder)
    {
        Destroy(foodOrder.gameObject);
        ReorganizeFoodOrder();
    }

    private void ReorganizeFoodOrder()
    {
        var childCount = foodOrdersDisplay.childCount - 1;
        for (int i = childCount; i >= 0; i--)
        {
            foodOrdersDisplay.GetChild(i).localPosition = new Vector3(0, (childCount - i) * newOrderOffet, 0);
        }
    }

    // Swap food order UI in multi-order ticket
    public void MoveUpFoodOrder()
    {
        if(foodOrdersDisplay.childCount > 1)
        {
            foodOrdersDisplay.GetChild(0).SetAsLastSibling();
            ReorganizeFoodOrder();
        }
    }
}
