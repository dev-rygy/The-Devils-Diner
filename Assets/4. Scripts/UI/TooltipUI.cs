using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI main;

    [SerializeField]
    private Button tooltipButton;
    [SerializeField]
    private GameObject uiHolder;
    [SerializeField]
    private TMP_Text toolTiptext;

    private UnityAction currentAction;

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(this);
    }

    public void Initialize(string tooltip, UnityAction onClickAction, bool pause)
    {
        if (pause)
            Time.timeScale = 0;

        uiHolder.SetActive(true);
        toolTiptext.text = tooltip;
        currentAction = onClickAction;
    }

    public void OnClick()
    {
        Time.timeScale = 1;

        uiHolder.SetActive(false);

        currentAction?.Invoke();
        currentAction = null;
    }
}
