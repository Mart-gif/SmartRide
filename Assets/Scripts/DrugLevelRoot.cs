using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragLevelRootUI : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] private RectTransform targetRect;
    [SerializeField] private float minX = -300f;
    [SerializeField] private float maxX = 300f;
    [SerializeField] private float minY = -600f;
    [SerializeField] private float maxY = 600f;

    private Vector2 lastPointerLocalPosition;

    private void Awake()
    {
        if (targetRect == null)
            targetRect = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransform parentRect = targetRect.parent as RectTransform;
        if (parentRect == null)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera,
            out lastPointerLocalPosition
        );
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransform parentRect = targetRect.parent as RectTransform;
        if (parentRect == null)
            return;

        Vector2 currentPointerLocalPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera,
            out currentPointerLocalPosition
        );

        Vector2 delta = currentPointerLocalPosition - lastPointerLocalPosition;
        Vector2 targetPos = targetRect.anchoredPosition + delta;

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        targetRect.anchoredPosition = targetPos;
        lastPointerLocalPosition = currentPointerLocalPosition;
    }
}
