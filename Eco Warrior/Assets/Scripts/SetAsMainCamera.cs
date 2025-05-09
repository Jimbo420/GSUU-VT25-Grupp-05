using UnityEngine;

public class SetAsMainCamera : MonoBehaviour
{
    void Start()
    {
        Camera.main.transform.position = transform.position + new Vector3(0, 0, -10f);  // Adjust Z
    }
}
