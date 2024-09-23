using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ticket : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private bool showTimerInEditor;

    [Header("Required Components")]
    [SerializeField]
    private StationSprites stationSprites;
    [SerializeField]
    private TextMeshProUGUI timeText;
    [SerializeField]
    private TextMeshProUGUI seatText;
    [SerializeField]
    private Image portraitImage;
    [SerializeField]
    private Image dishImage;
    [SerializeField]
    private Image stationImage;
    [SerializeField]
    private Image rugImage;
    [SerializeField]
    private Image ingredientImage_1;
    [SerializeField]
    private Image ingredientImage_2;
    [SerializeField]
    private Image ingredientImage_Single;
    [SerializeField]
    private Image ingredientBorder_1;
    [SerializeField]
    private Image ingredientBorder_2;
    [SerializeField]
    private Image ingredientBorder_Single;

    private float startTime;
    private TimeSpan timeLeft;
    private float timeTilAngry;
    private int seatNumber;
    private FoodData foodData;

    public float TimeTilAngry => timeTilAngry;
    public float TimeLeft => (float)timeLeft.TotalSeconds;

    private void Update()
    {
        timeLeft = TimeSpan.FromSeconds(timeTilAngry - (Time.time - startTime));
        if(showTimerInEditor) gameObject.name = $"Ticket({TimeLeft})";

        timeText.text = timeLeft.Minutes + ":" + timeLeft.Seconds.ToString("00");
        if (timeLeft.TotalSeconds > timeTilAngry / 2)
            timeText.color = Color.green;
        else if (timeLeft.TotalSeconds > 10 && timeLeft.TotalSeconds <= timeTilAngry / 2)
            timeText.color = Color.yellow;
        else if(timeLeft.TotalSeconds <= 10)
            timeText.color = Color.red;
    }


    public void InitializeTicketValues(Demon demon, FoodData foodData, int seatNumber, Sprite rugSprite)
    {
        portraitImage.sprite = demon.portrait;
        seatText.text = $"# {seatNumber + 1}";

        this.foodData = foodData;
        dishImage.sprite = foodData.PlatedSprite;
        stationImage.sprite = stationSprites.GetSprite(foodData.StationType);
        rugImage.sprite = rugSprite;

        startTime = Time.time;
        timeTilAngry = demon.TimeTilAngry / GameManager.main.TimeScale;
        timeLeft = TimeSpan.FromSeconds(timeTilAngry - (Time.time - startTime));
        this.seatNumber = seatNumber;

        var ingredients = foodData.requiredIngredients;
        if (ingredients.Length > 1)
        {
            ingredientImage_1.sprite = ingredients[0].Sprite;
            ingredientImage_2.sprite = ingredients[1].Sprite;
            ingredientImage_Single.enabled = false;

            ingredientBorder_1.enabled = true;
            ingredientBorder_2.enabled = true;
            ingredientBorder_Single.enabled = false;

        }
        else
        {
            ingredientImage_Single.sprite = ingredients[0].Sprite;
            ingredientImage_1.enabled = false;
            ingredientImage_2.enabled = false;

            ingredientBorder_1.enabled = false;
            ingredientBorder_2.enabled = false;
            ingredientBorder_Single.enabled = true;
        }
            
    }

    public bool Contains(int seatNumber, FoodData foodData)
    {
        return this.seatNumber == seatNumber && this.foodData == foodData;
    }
}
