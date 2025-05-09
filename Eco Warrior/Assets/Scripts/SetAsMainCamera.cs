using UnityEngine;

public class SetAsMainCamera : MonoBehaviour
{
    [SerializeField] private bool isPrimary = false; // Determines if this script should supersede others

    void Start()
    {
        if (!isPrimary) return; // Skip execution if this is not the primary script

        // Ensure this is the only script setting the main camera
        SetAsMainCamera[] scripts = FindObjectsOfType<SetAsMainCamera>();
        foreach (var script in scripts)
        {
            if (script != this && script.isPrimary)
            {
                Debug.LogWarning("Another primary SetAsMainCamera script already exists. Disabling this one.");
                enabled = false;
                return;
            }
        }

        // Set the camera position
        Camera.main.transform.position = transform.position + new Vector3(0, 0, -10f); // Adjust Z
    }
}
