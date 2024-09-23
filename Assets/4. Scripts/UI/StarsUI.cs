using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StarsUI : MonoBehaviour
{
    public static StarsUI main;

    // Star Objects
    [Header("Settings")]
    [SerializeField]
    private Sprite litStarSprite;
    [SerializeField]
    private Sprite dimStarSprite;
    [SerializeField]
    private Image[] starArray;

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateStar(GameManager.main.CurrentStar);
    }

    // Updates the image of a star when lost
    public void UpdateStar(int value)
    {
        for (int i = 0; i < GameManager.MAX_STAR_COUNT; i++)
        {
            if (i < value)
                starArray[i].sprite = litStarSprite;
            else
                starArray[i].sprite = dimStarSprite;
        }
    }
}
