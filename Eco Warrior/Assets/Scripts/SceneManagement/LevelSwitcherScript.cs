using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSwitcherScript : MonoBehaviour
{
    // Public field to set the scene name in the Unity Inspector
    [SerializeField]
    private string sceneToLoad;

    // Public field to set the key in the Unity Inspector
    [SerializeField]
    private KeyCode switchKey = KeyCode.None;

    void Update()
    {
        // Check if the specified key is pressed
        if (Input.GetKeyDown(switchKey))
        {
            SwitchLevel();
        }
    }

    // Method to switch the level
    private void SwitchLevel()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Scene name is not set or is empty!");
        }
    }
}
