using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreTracker : MonoBehaviour
{
    // Public score count
    public int score;
    
    // Text for UI
    public Text scoreText;

    public void Start()
    {
        // If the player is on level 1, the score resets to 0. Otherwise, it is obtained from PlayerPrefs.
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            PlayerPrefs.SetInt("score", 0);
        }
        else
        {
            score = PlayerPrefs.GetInt("score");
        }
        
        // Sets the score UI text to match the current score.
        scoreText.text = score.ToString();
    }

    // Sets and stores the player's current score and updates score UI text.
    public void SetScore(int newScore)
    {
        score = newScore;
        PlayerPrefs.SetInt("score", score);
        scoreText.text = score.ToString();
    }
}
