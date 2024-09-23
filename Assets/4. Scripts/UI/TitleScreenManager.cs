using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private ScoreData score;
    [SerializeField] private TMP_Text scoreText;

    private void Start()
    {
        //scoreText.text = "High Score: " + "\n" + score.HighScore.ToString().PadLeft(4, '0');
    }

    public void StartGame()
    {
        SceneManager.LoadScene(4);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToOptions()
    {
        SceneManager.LoadScene(3);
    }
}
