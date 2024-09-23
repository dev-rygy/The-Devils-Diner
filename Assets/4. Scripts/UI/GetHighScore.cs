using LootLocker.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetHighScore : MonoBehaviour
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

        StartCoroutine(WaitLeaderboard());
    }

    private IEnumerator WaitLeaderboard()
    {
       while (!leaderboardManager.IsReady)
            yield return null;

        leaderboardManager.GetScoreSingle(LeaderboardCallback);
    }

    public void LeaderboardCallback(LootLockerGetMemberRankResponse response)
    {
        textUI.text = preText +
            response.score.ToString(format);
    }
}
