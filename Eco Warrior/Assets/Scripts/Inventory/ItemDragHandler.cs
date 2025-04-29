using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform _originalParent;
    private CanvasGroup _canvasGroup;
    public static bool IsDragging { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _originalParent = transform.parent;
        Canvas canvas = FindFirstObjectByType<Canvas>();
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0.6f;
        IsDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);

        transform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true; //You can click on it again
        _canvasGroup.alpha = 1f; //No longer transparent
        Slot dropSlot = eventData.pointerEnter?.GetComponentInParent<Slot>();
        Slot originalSlot = _originalParent.GetComponent<Slot>();
        if (dropSlot != null)
        {
            if (dropSlot.CurrentItem != null)
            {
                // Swap items
                dropSlot.CurrentItem.transform.SetParent(originalSlot.transform);
                originalSlot.CurrentItem = dropSlot.CurrentItem;
                dropSlot.CurrentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
            else if (originalSlot != null)
            {
                originalSlot.CurrentItem = null;
            }

            transform.SetParent(dropSlot.transform);
            dropSlot.CurrentItem = gameObject;
        }
        else
        {
            Debug.Log("No slot detected");
            transform.SetParent(_originalParent);
        }
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero; //Centers in the slot
        IsDragging = false;
    }
}
