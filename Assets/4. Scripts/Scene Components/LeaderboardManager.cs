using LootLocker.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;
using static UnityEngine.Rendering.DebugUI;

public struct RankNameScore 
{
    public int Rank;
    public string Name;
    public int Score;
}

public class LeaderboardManager : MonoBehaviour
{
    private static string IDENTIFIER_KEY = "global_leaderboard";
    private static string UID_KEY = "UID";
    private static string NAME_KEY = "Name";

    public static LeaderboardManager main;

    [Header("Settings")]
    [SerializeField]
    private int maxEntries = 20;

    [Header("Debugs")]
    [SerializeField]
    private bool isReady = false;
    [SerializeField]
    private string uniqueID = "Unidentified";
    [SerializeField]
    private string playerName = "Unidentified";
    [SerializeField]
    private int localScore = 0;

    public bool IsReady => isReady;
    public int LocalScore => localScore;

    public UnityEvent<string> OnNameChange;
    public UnityEvent OnScoreChange;

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(gameObject);

        Initialize();
    }

    private void Start()
    {
        
    }

    [ContextMenu("Initialized")]
    private void Initialize()
    {
        PopulateIDAndName();

        LootLockerSDKManager.StartSession("Session", (response) =>
        {
            if (response.success)
            {
                Debug.Log("Session started successfully");
                isReady = true;
            }
            else
            {
                Debug.Log("Failed: " + response.Error);
            }
        });
    }

    public void PopulateIDAndName()
    {
        if (PlayerPrefs.HasKey(UID_KEY))
        {
            uniqueID = PlayerPrefs.GetString(UID_KEY);
            playerName = PlayerPrefs.GetString(NAME_KEY);
        }
        else
        {
            uniqueID = SystemInfo.deviceUniqueIdentifier;
            playerName = "Chef" + UnityEngine.Random.Range(1, 9999).ToString("0000");

            PlayerPrefs.SetString(UID_KEY, uniqueID);
            PlayerPrefs.SetString(NAME_KEY, playerName);
        }

        OnNameChange?.Invoke(playerName);

        Debug.Log($"ID: {uniqueID}, Name: {playerName}");
    }

    [ContextMenu("Add random entry")]
    private void AddRandomEntry()
    {
        var name = "Chef" + UnityEngine.Random.Range(1, 9999).ToString("0000");
        var testID = name + UnityEngine.Random.Range(1, 9999);
        var value = UnityEngine.Random.Range(1, 9999999);

        LootLockerSDKManager.SubmitScore(testID, value, IDENTIFIER_KEY, name, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Submit score successful");
            }
            else
            {
                Debug.Log("Failed: " + response.Error);
            }
        });
    }

    /*[ContextMenu("Update Name")]
    private void ChangeNameUI()
    {
        ChangeName(playerName);
    }*/

    public void SubmitName(string newName)
    {
        GetScoreSingle(SubmitNameCallback);

        playerName = newName;
        PlayerPrefs.SetString(NAME_KEY, playerName);

        OnNameChange?.Invoke(playerName);
    }

    private void SubmitNameCallback(LootLockerGetMemberRankResponse response)
    {
        SetScore(response.score);
    }

    public string GetName()
    {
        return playerName;
    }

    public void GetScoreList(Action<LootLockerGetScoreListResponse> Callback)
    {
        LootLockerSDKManager.GetScoreList(IDENTIFIER_KEY, maxEntries, 0, (response) =>
        {
            if (response.success)
            {
                Debug.Log($"GetScoreList successful, count: {response.items.Length}");
                Callback(response);
            }
            else
            {
                Debug.Log("Failed: " + response.Error);
            }
        });
    }

    [ContextMenu("Get Score Single")]
    public void GetScoreSingle(Action<LootLockerGetMemberRankResponse> Callback)
    {
        GetScoreSingle(uniqueID, Callback);
    }

    private void GetScoreSingle(string uniqueID, Action<LootLockerGetMemberRankResponse> Callback)
    {
        LootLockerSDKManager.GetMemberRank(IDENTIFIER_KEY, uniqueID, (response) =>
        {
            if (response.success)
            {
                Debug.Log($"Successful: Name: {response.metadata}, Score: {response.score}," +
                    $" ID: {response.member_id}");

                Callback(response);
            }
            else
            {
                Debug.Log("Failed: " + response.Error);
                Callback(null);
            }
        });
    }

    [ContextMenu("Submit Score Test")]
    private void SetScore()
    {
        SetScore(-1);
    }

    private void SetScore(int value)
    {
        LootLockerSDKManager.SubmitScore(uniqueID, value, IDENTIFIER_KEY, playerName, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Submit score successful");

                OnScoreChange?.Invoke();
            }
            else
            {
                Debug.Log("Failed: " + response.Error);
            }
        });
    }

    public void SubmitScore(int value)
    {
        localScore = value;
        SetScore(value);
    }
}
