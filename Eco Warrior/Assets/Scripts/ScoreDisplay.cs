using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI enemiesKilledNumbersText;
    public TextMeshProUGUI objectivesCompletedNumbersText;
    public TextMeshProUGUI timesDetectedNumbersText;
    public TextMeshProUGUI gameScoreText;

    void Update()
    {
        if (ScoreManager.Instance != null)
        {
            enemiesKilledNumbersText.text = "-100 x " + ScoreManager.Instance.enemiesKilled;
            objectivesCompletedNumbersText.text = "1000 x " + ScoreManager.Instance.objectivesCompleted;
            timesDetectedNumbersText.text = "-200 x " + ScoreManager.Instance.timesDetected;
            gameScoreText.text = "Score: " + (ScoreManager.Instance.enemiesKilled * -100 + ScoreManager.Instance.objectivesCompleted * 1000 + ScoreManager.Instance.timesDetected * -200);
        }
    }
}
