using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoundManager))]
public class SoundManagerEditor : Editor
{
    private SoundManager soundManager;

    private void OnEnable()
    {
        soundManager = (SoundManager) target;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Test"))
            soundManager.Test();

        DrawDefaultInspector();
    }
}
