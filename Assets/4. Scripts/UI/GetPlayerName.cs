using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetPlayerName : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private string preText = "";

    private TextMeshProUGUI textUI;
    private LeaderboardManager leaderboardManager;

    private void OnDisable()
    {
        leaderboardManager?.OnNameChange.RemoveListener(OnNameChange);
    }

    private void Start()
    {
        textUI = GetComponent<TextMeshProUGUI>();
        StartCoroutine(WaitLeaderboard());
    }

    IEnumerator WaitLeaderboard()
    {
        while(leaderboardManager == null)
        {
            leaderboardManager = LeaderboardManager.main;
            yield return null;
        }

        leaderboardManager.OnNameChange.AddListener(OnNameChange);
        leaderboardManager.PopulateIDAndName();
    }

    private void OnNameChange(string newName)
    {
        textUI.text = preText + newName;
    }
}
