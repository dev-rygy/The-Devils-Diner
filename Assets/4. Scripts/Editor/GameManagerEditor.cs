using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    private GameManager GameManager;

    private void OnEnable()
    {
        GameManager = (GameManager)target;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Reset Game Save"))
            GameManager.ResetGameSave();

        GUILayout.Space(2);

        DrawDefaultInspector();
    }
}
