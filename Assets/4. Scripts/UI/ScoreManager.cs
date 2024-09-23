using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager main;

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(this);
    }

    public ScoreData score;
    [SerializeField]
    private TMP_Text scoreText;
    [SerializeField]
    private TMP_Text DisplayHSR;

    private void Start()
    {
        if (score.Get() > score.HighScore) score.HighScore = score.Get();

        if (DisplayHSR != null) DisplayHSR.enabled = false;

        if (SceneManager.GetActiveScene().name == "GameOver")
        {
            scoreText.text = "High Score: " + score.HighScore.ToString().PadLeft(4, '0');
        }
        else
        {
            scoreText.text = score.Get().ToString().PadLeft(4, '0');
        }
    }

    public void UpdateScore()
    {
        if (SceneManager.GetActiveScene().name == "Main")
        {
            score.Increment();

            if (score.score > score.HighScore && score.HighScore != 0)
            {
                DisplayHSR.enabled = true;
            }

            scoreText.text = score.Get().ToString().PadLeft(4, '0');
        }
        else if(SceneManager.GetActiveScene().name == "GameOver")
        {
            scoreText.text = "High Score: " + score.HighScore.ToString().PadLeft(4, '0');
        }
    }
}
