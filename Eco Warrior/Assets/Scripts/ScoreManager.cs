using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int enemiesKilled = 0;
    public int objectivesCompleted = 0;
    public int timesDetected = 0;

    private void Awake()
    {
        // Singleton-pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps ScoreManager between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
    }

    public void ObjectiveCompleted()
    {
        objectivesCompleted++;
    }

    public void TimesDetected()
    {
        timesDetected++;
    }
}
