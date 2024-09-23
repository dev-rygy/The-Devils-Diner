using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField]
    private int maxEntries = 20;
    [SerializeField]
    private GameObject entryPrefab;

    [SerializeField]
    private Transform contentTransform;

    private LeaderboardManager leaderboardManager;

    // Start is called before the first frame update
    private void Awake()
    {
        leaderboardManager = LeaderboardManager.main;
    }

    public void Activate()
    {
        leaderboardManager.OnScoreChange.AddListener(Refresh);
    }

    public void DeActivate()
    {
        leaderboardManager.OnScoreChange.RemoveListener(Refresh);
    }

    public void Refresh()
    {
        foreach(Transform child in contentTransform)
            Destroy(child.gameObject);

        leaderboardManager.GetScoreList(GetScoreListCallback);
    }

    private void GetScoreListCallback(LootLockerGetScoreListResponse response)
    {
        var items = response.items;
        var rankLength = items[0].rank.ToString().Length + 1;

        StringBuilder format = new StringBuilder();
        for (int j = 0; j < rankLength; j++)
        {
            format.Append("0");

        }

        int i = 0;
        var length = items.Length < maxEntries ? items.Length : maxEntries;
        for (; i < length; i++)
        {
            var entryGO = Instantiate(entryPrefab, contentTransform);
            entryGO.GetComponent<LeaderboardEntry>().SetText(format.ToString(), items[i].rank,
                items[i].metadata, items[i].score);
        }
        var lastRank = items[length - 1].rank;
        if (i < maxEntries - 1)
        {
            var count = 1;
            for (; i < maxEntries; i++)
            {
                var entryGO = Instantiate(entryPrefab, contentTransform);
                entryGO.GetComponent<LeaderboardEntry>().SetText(format.ToString(),
                    lastRank + count, "None", 0);
                count++;
            }
        }
    }
}
