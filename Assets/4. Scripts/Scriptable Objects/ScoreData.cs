using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Score Data", menuName = "Scriptable Objects/Score")]
public class ScoreData : ScriptableObject
{
    public int score;
    public int increment;
    public int HighScore;

    private void OnDestroy()
    {
        score = 0;
    }

    private void Awake()
    {
        score = 0;
    }

    public void Increment()
    {
        score+=increment;
    }

    public void Set(int newScore)
    {
        score = newScore;
    }

    public int Get()
    {
        return score;
    }
}