using UnityEngine;

public class LoseScreen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScores(int finalScore, int highScore)
    {
        var scoreText = transform.Find("Label_FinalScore").GetComponent<UnityEngine.UI.Text>();
        var highScoreText = transform.Find("Label_HighScore").GetComponent<UnityEngine.UI.Text>();
        if (scoreText != null)
        {
            scoreText.text = "Final Score: " + finalScore.ToString();
        }
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore.ToString();
        }
    }
}
