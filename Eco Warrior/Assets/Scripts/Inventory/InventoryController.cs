using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private GameObject _slotPrefab;

    [SerializeField] int _slotCount;
    [SerializeField] private GameObject[] itemPrefabs;
    private List<Slot> _slots = new List<Slot>();
    public List<Slot> GetSlots() => _slots;

    private static InventoryController _instance;

    private void Awake()
    {
        // Singleton-pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instance
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); // Keeps InventoryController between scenes
    }

    void Start()
    {
        for (int i = 0; i < _slotCount; i++)
        {
            Slot slot = Instantiate(_slotPrefab, _inventoryPanel.transform).GetComponent<Slot>();
            if (i < itemPrefabs.Length)
            {
                GameObject item = Instantiate(itemPrefabs[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.CurrentItem = item;
            }
            _slots.Add(slot);
        }
    }

    public GameObject TryAddItem(GameObject itemPrefab)
    {
        foreach (Slot slot in _slots)
        {
            if (slot.CurrentItem == null)
            {
                GameObject item = Instantiate(itemPrefab, slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.CurrentItem = item;
                return item;
            }
        }
        return null;
    }
}
