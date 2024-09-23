using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlScheme
{
    Joystick,
    PointNClick,
    DragOnScreen
}

public class ControlSchemeManager: MonoBehaviour
{
    public static ControlSchemeManager main;

    private string CONTROL_SCHEME_KEY = "CONTROL_SCHEME";
    private string JOYSTICK_VALUE = "JOYSTICK";
    private string POINT_N_CLICK_VALUE = "POINT_N_CLICK";
    private string DRAG_ON_SCREEN_VALUE = "DRAG_ON_SCREEN";

    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private GameObject joystickUI;

    public ControlScheme CurrentControlScheme { get; private set; }

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        ControlScheme controlScheme = GetControlScheme();
        SetControlScheme(controlScheme);
    }

    public void SetControlScheme(ControlScheme controlScheme)
    {
        PlayerPrefs.SetString(CONTROL_SCHEME_KEY, GetStringFromEnum(controlScheme));
        playerController.SetControlScheme(controlScheme);
        joystickUI.SetActive(controlScheme == ControlScheme.Joystick);

        CurrentControlScheme = controlScheme;
    }

    public ControlScheme GetControlScheme()
    {
        if (!PlayerPrefs.HasKey(CONTROL_SCHEME_KEY))
            PlayerPrefs.SetString(CONTROL_SCHEME_KEY, DRAG_ON_SCREEN_VALUE);

        string controlScheme = PlayerPrefs.GetString(CONTROL_SCHEME_KEY);
        if (controlScheme == JOYSTICK_VALUE)
            return ControlScheme.Joystick;
        else if (controlScheme == POINT_N_CLICK_VALUE)
            return ControlScheme.PointNClick;
        else if (controlScheme == DRAG_ON_SCREEN_VALUE)
            return ControlScheme.DragOnScreen;

        Debug.LogWarning("Control scheme was not correctly initialized");
        return ControlScheme.DragOnScreen;
    }

    private string GetStringFromEnum(ControlScheme controlScheme)
    {
        switch (controlScheme)
        {
            case ControlScheme.Joystick:
                return JOYSTICK_VALUE;
            case ControlScheme.PointNClick:
                return POINT_N_CLICK_VALUE;
            case ControlScheme.DragOnScreen:
                return DRAG_ON_SCREEN_VALUE;
            default:
                return null;
        }
    }
}
