using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NameChangeUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI promptUI;
    [SerializeField]
    private TMP_InputField inputField;

    public void Initialize()
    {
        inputField.text = LeaderboardManager.main.GetName();
    }

    public void Confirm() 
    {
        LeaderboardManager.main.SubmitName(inputField.text);
        PopupMenuUI.main.OpenLeaderboardUI();
    }

    public void Random()
    {
        inputField.text = "Chef" + UnityEngine.Random.Range(1, 9999).ToString("0000");
    }

}
