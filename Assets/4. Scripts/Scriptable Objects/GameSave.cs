using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Save", menuName = "Scriptable Objects/Game Save")]
public class GameSave : ScriptableObject
{
    public int currentNight;
    public int currentScore;

    public void Reset()
    {
        currentNight = 0;
        currentScore = 0;
    }
}
