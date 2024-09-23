using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishWasher : Interactable
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.main;
    }

    public override void Interact(PlayerInteraction playerInteraction)
    {
        if (playerInteraction.CurrentDish?.CurrentFood == null)
        {
            playerInteraction.ConsumeDish();
            gameManager.WashOneDish();
        }
        else
        {
            Debug.Log("Dish still has food, get rid of it");
        }   
    }
}
