using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetLocalScore : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private string preText = "";
    [SerializeField]
    private string format = "000";

    private TextMeshProUGUI textUI;
    private LeaderboardManager leaderboardManager;

    // Start is called before the first frame update
    void Start()
    {
        textUI = GetComponent<TextMeshProUGUI>();
        leaderboardManager = LeaderboardManager.main;

        textUI.text = preText + leaderboardManager.LocalScore.ToString(format);
    }
}
