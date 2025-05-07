using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI enemiesKilledText;
    public TextMeshProUGUI objectivesCompletedText;

    void Update()
    {
        if (ScoreManager.Instance != null)
        {
            enemiesKilledText.text = "Enemies killed: " + ScoreManager.Instance.enemiesKilled;
            objectivesCompletedText.text = "Objectives completed: " + ScoreManager.Instance.objectivesCompleted;
        }
    }
}
