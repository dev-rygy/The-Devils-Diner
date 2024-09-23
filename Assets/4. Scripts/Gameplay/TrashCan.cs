using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : Interactable
{
    public override void Interact(PlayerInteraction playerInteraction)
    {
        playerInteraction.CurrentDish?.MakeDirty();
        playerInteraction.ConsumeIngredient();
    }
}
