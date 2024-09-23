using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PopupMenuUI))]
public class PopupMenuUIEditor : Editor
{
    private PopupMenuUI popupMenu;

    private void OnEnable()
    {
        popupMenu = (PopupMenuUI)target;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Open"))
                popupMenu.Open();
            else if (GUILayout.Button("Close"))
                popupMenu.CloseUI();
        }
        GUILayout.EndHorizontal();

        foreach (PopupMenuType menuType in System.Enum.GetValues(typeof(PopupMenuType)))
        {
            if (GUILayout.Button($"Show {menuType.ToString()} UI"))
            {
                switch (menuType)
                {
                    case PopupMenuType.Options:
                        popupMenu.OpenOptionsUI();
                        break;
                    case PopupMenuType.Sound:
                        popupMenu.OpenSoundUI();
                        break;
                    case PopupMenuType.Control:
                        popupMenu.OpenControlUI();
                        break;
                    case PopupMenuType.Leaderboard:
                        popupMenu.OpenLeaderboardUI();
                        break;
                    case PopupMenuType.NameChange:
                        popupMenu.OpenNameChangeUI();
                        break;
                }
            }
        }

        GUILayout.Space(10);
        DrawDefaultInspector();
    }
}
