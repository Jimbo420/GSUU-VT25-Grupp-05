using UnityEngine;

public class KnockOut : MonoBehaviour
{
    private bool _isInRange;
    private TargetPlayer _targetPlayer;
    public GameObject textObject;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_targetPlayer.hasLineOfSight|| !other.CompareTag("Player")) return;
        textObject.SetActive(true);
        _isInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        textObject.SetActive(false);
        _isInRange = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!_isInRange) return;
        if (!Input.GetKey(KeyCode.F)) return;
        Destroy(gameObject);
    }

    void Start()
    {
       _targetPlayer = GetComponent<TargetPlayer>();
    }
}
