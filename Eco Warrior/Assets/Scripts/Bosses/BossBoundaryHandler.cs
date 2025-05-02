using UnityEngine;

public class BossBoundaryHandler : MonoBehaviour
{
    [Header("Boundary Settings")]
    public float boundaryRadius = 20f;
    public Vector3 startingLocation;

    public void CheckBoundary()
    {
        float distanceFromStart = Vector3.Distance(transform.position, startingLocation);
        if (distanceFromStart > boundaryRadius)
        {
            transform.position = startingLocation;
        }
    }
}
