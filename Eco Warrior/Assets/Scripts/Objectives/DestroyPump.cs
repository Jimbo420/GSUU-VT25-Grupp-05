using Assets.Scripts.Objectives;
using UnityEngine;
using UnityEngine.UI;

public class DestroyPump : MonoBehaviour, IDestroy
{
    public GameObject textObject;
    [SerializeField] private Sprite _destroyedSprite;
    private DestroyedObjects _destroyedObjects;
    private Slider _slider;
    private bool _isInRange;
    private bool _isDestroyed;

    void Awake()
    {
        _slider = GetComponentInChildren<Slider>();
        _destroyedObjects = FindObjectOfType<DestroyedObjects>();
    }

    void FixedUpdate()
    {
        if (!_isInRange) return;
        if (!Input.GetKey(KeyCode.F)) return;

        _slider.value -= 0.1f;

        if (_slider.value == 0 && !_isDestroyed)
            Destroy();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDestroyed || !other.CompareTag("Player")) return;
        textObject.SetActive(true);
        _isInRange = true;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        textObject.SetActive(false);
        _isInRange = false;
    }

    public void Destroy()
    {
        ParticleSystem ps = transform.parent.GetComponentInChildren<ParticleSystem>();
        if(ps is not null) ps.Play();
        _slider.gameObject.SetActive(false);
        textObject.gameObject.SetActive(false);

        if (GetComponent<Animator>() != null)
        {
            GetComponent<Animator>().enabled = false;
        }

        GetComponent<SpriteRenderer>().sprite = _destroyedSprite;

        if (this.CompareTag("Computer"))
        {
            GetComponent<DoorOpener>().OpenWeaponRoomDoors();
        }

        _destroyedObjects._gameObjectsDestroyed++;
        _isDestroyed = true;
        ScoreManager.Instance.ObjectiveCompleted();
    }
}
