using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class LeaderboardEntry : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI leftText;
    [SerializeField]
    private TextMeshProUGUI rightText;

    public void SetText(string format, int rank, string name, int score)
    {
        leftText.text = $"{rank.ToString(format)}.{name}";
        rightText.text = score.ToString("000000");
    }
}
