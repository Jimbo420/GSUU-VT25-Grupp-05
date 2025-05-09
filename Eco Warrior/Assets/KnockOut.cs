using UnityEngine;
using UnityEngine.AI;

public class KnockOut : MonoBehaviour
{
    private bool _isInRange;
    private TargetPlayer _targetPlayer;
    public GameObject textObject;
    [SerializeField] private GameObject _sleep;
    [SerializeField] private Sprite _sprite;
    public bool IsKnocked;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_targetPlayer.hasLineOfSight || IsKnocked || !other.CompareTag("Player")) return;
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
        KnockOutEnemy();
    }

    void Start()
    {
       _targetPlayer = GetComponent<TargetPlayer>();
    }

    void KnockOutEnemy()
    {
        IsKnocked = true;
        textObject.SetActive(false);

        var components = GetComponents<MonoBehaviour>();
        foreach (var comp in components)
            if (comp != this && comp.GetType() != typeof(EnemyMovement))
                comp.enabled = false;
        

        GetComponent<Animator>().enabled = false; ;

        GetComponent<NavMeshAgent>().enabled = false;

        Collider2D parentCollider = GetComponentInParent<Collider2D>();
        if (parentCollider != null)
        {
            parentCollider.enabled = false;
        }

        foreach (Transform child in transform)
            child.gameObject.SetActive(false);

        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<SpriteRenderer>().sprite = _sprite;
        _sleep.gameObject.SetActive(true);


    }
}
