using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public enum PopupMenuType
{
    Options,
    Sound,
    Control,
    Leaderboard,
    NameChange,
}

public class PopupMenuUI : MonoBehaviour
{
    public static PopupMenuUI main;

    [Header("Essential")]
    [SerializeField]
    private GameObject uiHolder;
    [SerializeField]
    private TMP_Text titleText;
    [SerializeField]
    private GameObject backButton;
    [SerializeField]
    private TMP_Text backButtonText;

    [Header("UI groups")]
    [SerializeField]
    private GameObject optionsUI;
    [SerializeField]
    private GameObject soundUI;
    [SerializeField]
    private SoundUI soundUIScript;
    [SerializeField]
    private GameObject controlUI;
    [SerializeField]
    private ControlUI controlUIScript;
    [SerializeField]
    private GameObject leaderboardUI;
    [SerializeField]
    private LeaderboardUI leaderboardScript;
    [SerializeField]
    private GameObject nameChangeUI;
    [SerializeField]
    private NameChangeUI nameChangeScript;

    [Header("Debugs")]
    [SerializeField]
    private PopupMenuType currentMenuType;

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(this);
    }

    [ContextMenu("Open")]
    public void Open()
    {
        OpenOptionsUI();
    }

    private void OnOpenUI()
    {
        Time.timeScale = 0;

        uiHolder.SetActive(true);
    }

    [ContextMenu("Close")]
    public void CloseUI()
    {
        Time.timeScale = 1;

        uiHolder.SetActive(false);
    }

    [ContextMenu("Show OptionUI")]
    public void OpenOptionsUI()
    {
        OnOpenUI();
        CloseAllUI();
        currentMenuType = PopupMenuType.Options;
        optionsUI.SetActive(true);
        titleText.text = "Options";
        backButtonText.text = "Close";
    }

    [ContextMenu("Show SoundUI")]
    public void OpenSoundUI()
    {
        OnOpenUI();
        CloseAllUI();
        currentMenuType = PopupMenuType.Sound;
        soundUI.SetActive(true);
        titleText.text = "Sound";
        backButtonText.text = "Back";

        soundUIScript.Initialize();
    }

    [ContextMenu("Show ControlUI")]
    public void OpenControlUI()
    {
        OnOpenUI();
        CloseAllUI();
        currentMenuType = PopupMenuType.Control;
        controlUI.SetActive(true);
        titleText.text = "Control";
        backButtonText.text = "Back";

        controlUIScript.Initialize();
    }

    [ContextMenu("Show LeaderboardUI")]
    public void OpenLeaderboardUI()
    {
        OnOpenUI();
        CloseAllUI();
        currentMenuType = PopupMenuType.Leaderboard;
        leaderboardUI.SetActive(true);
        titleText.text = "Leaderboard";
        backButtonText.text = "Close";

        leaderboardScript.Refresh();
        leaderboardScript.Activate();
    }

    [ContextMenu("Show NameChangeUI")]
    public void OpenNameChangeUI()
    {
        OnOpenUI();
        CloseAllUI();
        currentMenuType = PopupMenuType.NameChange;
        nameChangeUI.SetActive(true);
        titleText.text = "Name Change";
        backButton.SetActive(false);

        nameChangeScript.Initialize();
    }

    private void CloseAllUI()
    {
        optionsUI.SetActive(false);
        soundUI.SetActive(false);
        controlUI.SetActive(false);
        leaderboardUI.SetActive(false);
        nameChangeUI.SetActive(false);
        backButton.SetActive(true);
    }

    public void HandleBackButtonClick()
    {
        if (currentMenuType == PopupMenuType.Options)
        {
            CloseUI();
        }
        else if (currentMenuType == PopupMenuType.Leaderboard)
        {
            leaderboardScript.DeActivate();
            CloseUI();
        }
        else
        {
            OpenOptionsUI();
        }
    }
}
