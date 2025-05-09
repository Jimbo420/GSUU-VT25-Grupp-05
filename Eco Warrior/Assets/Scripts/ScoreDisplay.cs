using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI enemiesKilledText;
    public TextMeshProUGUI objectivesCompletedText;
    public TextMeshProUGUI timesDetectedText;
    public TextMeshProUGUI gameScoreText;

    void Update()
    {
        if (ScoreManager.Instance != null)
        {
            enemiesKilledText.text = "-100 x " + ScoreManager.Instance.enemiesKilled;
            objectivesCompletedText.text = "1000 x " + ScoreManager.Instance.objectivesCompleted;
            timesDetectedText.text = "-200 x " + ScoreManager.Instance.timesDetected;
            gameScoreText.text = "Score: " + ScoreManager.Instance.enemiesKilled * -100 + ScoreManager.Instance.objectivesCompleted * 1000 + ScoreManager.Instance.timesDetected * -200;
        }
    }
}
