using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI enemiesKilledText;
    public TextMeshProUGUI objectivesCompletedText;
    public TextMeshProUGUI gameScoreText;

    void Update()
    {
        if (ScoreManager.Instance != null)
        {
            enemiesKilledText.text = "Enemies killed: " + ScoreManager.Instance.enemiesKilled;
            objectivesCompletedText.text = "Objectives completed: " + ScoreManager.Instance.objectivesCompleted;
            gameScoreText.text = "Score: " + ScoreManager.Instance.enemiesKilled * 100 + ScoreManager.Instance.objectivesCompleted * 1000;
        }
    }
}
