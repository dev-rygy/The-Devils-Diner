using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class ReadjustUIHandler : MonoBehaviour
{
    private static string MOVEMENT_X = "movementX";
    private static string MOVEMENT_Y = "movementY";
    private static string INTERACT_X = "interactX";
    private static string INTERACT_Y = "interactY"; 
    private static string REMOVE_X = "removeX";
    private static string REMOVE_Y = "removeY";

    [Header("Settings")]
    [SerializeField]
    private RectTransform movementRect;
    [SerializeField]
    private RectTransform interactRect;
    [SerializeField]
    private RectTransform removeRect;

    [Header("Default Positions")]
    [SerializeField]
    private float default_movementX = 0;
    [SerializeField]
    private float default_movementY = 0;
    [SerializeField]
    private float default_interactX = 0;
    [SerializeField]
    private float default_interactY = 0;
    [SerializeField]
    private float default_removeX = 0;
    [SerializeField]
    private float default_removeY = 0;

    [Header("Required Components")]
    [SerializeField]
    private OnScreenControl[] components;
    [SerializeField]
    private DragHandler[] dragHandlers;
    [SerializeField]
    private Image[] moveImages;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(MOVEMENT_X) && PlayerPrefs.HasKey(MOVEMENT_Y))
            movementRect.anchoredPosition = new Vector2(PlayerPrefs.GetFloat(MOVEMENT_X), PlayerPrefs.GetFloat(MOVEMENT_Y));

        if (PlayerPrefs.HasKey(INTERACT_X) && PlayerPrefs.HasKey(INTERACT_Y))
            interactRect.anchoredPosition = new Vector2(PlayerPrefs.GetFloat(INTERACT_X), PlayerPrefs.GetFloat(INTERACT_Y));

        if (PlayerPrefs.HasKey(REMOVE_X) && PlayerPrefs.HasKey(REMOVE_Y))
            removeRect.anchoredPosition = new Vector2(PlayerPrefs.GetFloat(REMOVE_X), PlayerPrefs.GetFloat(REMOVE_Y)); 
    }

    public void Adjust(bool isAdjusting)
    {
        foreach (var component in components)
            component.enabled = !isAdjusting;

        foreach(var image in moveImages)
            image.enabled = isAdjusting;

        foreach (var handler in dragHandlers)
            handler.IsDraggable = isAdjusting;
    }

    [ContextMenu("Get Default Position")]
    private void GetDefaultPosition()
    {
        default_movementX = movementRect.anchoredPosition.x;
        default_movementY = movementRect.anchoredPosition.y;
        default_interactX = interactRect.anchoredPosition.x;
        default_interactY = interactRect.anchoredPosition.y;
        default_removeX = removeRect.anchoredPosition.x;
        default_removeY = removeRect.anchoredPosition.y;
    }

    [ContextMenu("Reset To Default Position")]
    private void ResetToDefaultPosition()
    {
        movementRect.anchoredPosition = new Vector2(default_movementX, default_movementY);
        interactRect.anchoredPosition = new Vector2(default_interactX,default_interactY);
        removeRect.anchoredPosition = new Vector2(default_removeX,default_removeY);

        ConfirmAdjustment();
    }

    public void ConfirmAdjustment()
    {
        PlayerPrefs.SetFloat(MOVEMENT_X, movementRect.anchoredPosition.x);
        PlayerPrefs.SetFloat(MOVEMENT_Y, movementRect.anchoredPosition.y);
        PlayerPrefs.SetFloat(INTERACT_X, interactRect.anchoredPosition.x);
        PlayerPrefs.SetFloat(INTERACT_Y, interactRect.anchoredPosition.y);
        PlayerPrefs.SetFloat(REMOVE_X, removeRect.anchoredPosition.x);
        PlayerPrefs.SetFloat(REMOVE_Y, removeRect.anchoredPosition.y);
    }

    [ContextMenu("Clear Adjustment")]
    private void ClearAdjustment()
    {
        PlayerPrefs.DeleteKey(MOVEMENT_X);
        PlayerPrefs.DeleteKey(MOVEMENT_Y);
        PlayerPrefs.DeleteKey(INTERACT_X);
        PlayerPrefs.DeleteKey(INTERACT_Y);
        PlayerPrefs.DeleteKey(REMOVE_X);
        PlayerPrefs.DeleteKey(REMOVE_Y);
    }
}
