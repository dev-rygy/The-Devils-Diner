using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlUI : MonoBehaviour
{
    [SerializeField]
    private ReadjustUIHandler readjustUIHandler;
    [SerializeField]
    private GameObject readjustButton;

    public void Initialize()
    {
        readjustButton.SetActive(readjustUIHandler);
    }

    public void StartAdjustment()
    {
        if (readjustUIHandler == null) return;

        readjustUIHandler.Adjust(true);

        PopupMenuUI.main.CloseUI();
        TooltipUI.main.Initialize("Click here to confirm adjustment", ConfirmAdjustment, true);
    }

    private void ConfirmAdjustment()
    {
        if (readjustUIHandler == null) return;

        readjustUIHandler.Adjust(false);
        readjustUIHandler.ConfirmAdjustment();

        PopupMenuUI.main.OpenControlUI();
    }

    public void JoystickButton()
    {
        ControlSchemeManager.main.SetControlScheme(ControlScheme.Joystick);
        PopupMenuUI.main.CloseUI();
    }

    public void PointNClickButton()
    {
        ControlSchemeManager.main.SetControlScheme(ControlScheme.PointNClick);
        PopupMenuUI.main.CloseUI();
    }

    public void DragOnScreenButton()
    {
        ControlSchemeManager.main.SetControlScheme(ControlScheme.DragOnScreen);
        PopupMenuUI.main.CloseUI();
    }
}
