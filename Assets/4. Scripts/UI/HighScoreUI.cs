using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreUI : MonoBehaviour
{
    public static HighScoreUI main;

    private TextMeshProUGUI highScoreText;

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
        highScoreText = GetComponent<TextMeshProUGUI>();
    }

    [ContextMenu("Add 500 points")]
    private void Add500Points()
    {
        UpdateValue(500);
    }

    [ContextMenu("Add 5000 points")]
    private void Add5000Points()
    {
        UpdateValue(5000);
    }

    [ContextMenu("Add 50000 points")]
    private void Add50000Points()
    {
        UpdateValue(50000);
    }

    public void UpdateValue(int value)
    {
        highScoreText.text = (int.Parse(highScoreText.text) + value).ToString("0000");
    }
}
