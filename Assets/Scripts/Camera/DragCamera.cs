using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DragCamera : MonoBehaviour
{
    [Header("Limits")]
    public Vector2 minPosition;
    public Vector2 maxPosition;

    private Vector3 dragOrigin;
    private bool isDragging;

    void Update()
    {
#if UNITY_EDITOR
        HandleMouse();
#else
        HandleTouch();
#endif
    }

    void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 difference = dragOrigin - currentPos;

            MoveCamera(difference);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    void HandleTouch()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            dragOrigin = Camera.main.ScreenToWorldPoint(touch.position);
            isDragging = true;
        }

        if (touch.phase == TouchPhase.Moved && isDragging)
        {
            Vector3 currentPos = Camera.main.ScreenToWorldPoint(touch.position);
            Vector3 difference = dragOrigin - currentPos;

            MoveCamera(difference);
        }

        if (touch.phase == TouchPhase.Ended)
        {
            isDragging = false;
        }
    }

    void MoveCamera(Vector3 delta)
    {
        Vector3 newPos = transform.position + delta;

        newPos.x = Mathf.Clamp(newPos.x, minPosition.x, maxPosition.x);
        newPos.y = Mathf.Clamp(newPos.y, minPosition.y, maxPosition.y);

        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }
}
