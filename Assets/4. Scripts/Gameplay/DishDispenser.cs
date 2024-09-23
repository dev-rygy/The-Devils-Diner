using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DishDispenser : Interactable
{
    [Header("Settings")]
    [SerializeField]
    private GameObject dishPrefab;
    [SerializeField]
    private AudioClip sfx;

    [Header("Required Components")]
    [SerializeField]
    private TMP_Text countText;

    private GameManager gameManager;
    private int capacity;

    private void Start()
    {
        gameManager = GameManager.main;
        gameManager.OnDishCountChange.AddListener(OnDishCountChange);
        capacity = gameManager.DishCapacity;
        OnDishCountChange(gameManager.CurrentDishCount);      
    }

    public override void Interact(PlayerInteraction playerInteraction)
    {
        if (playerInteraction.IsHoldingItem) return;

        if (!gameManager.TakeOneDish())
        {
            StopAllCoroutines();
            StartCoroutine(FlashRedCoroutine());
            return;
        }

        var dishObj = Instantiate(dishPrefab, transform.position, Quaternion.identity);
        var dish = dishObj.GetComponent<Dish>();

        SoundManager.main.PlayOneShot(sfx);
        dish.Interact(playerInteraction);
    }

    private IEnumerator FlashRedCoroutine()
    {
        var count = 0;
        while(count < 2)
        {
            countText.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            countText.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            count++;
        }
    }

    public void OnDishCountChange(int count)
    {
        countText.text = $"{count}/{capacity}";
    }
}
