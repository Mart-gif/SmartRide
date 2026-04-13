using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraFix : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;

    [Header("Movement")]
    [SerializeField] private bool allowHorizontal = true;
    [SerializeField] private bool allowVertical = false;

    [Header("Bounds")]
    [SerializeField] private bool useBounds = true;
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -10f;
    [SerializeField] private float maxY = 10f;

    [Header("Feel")]
    [SerializeField] private float dragSpeed = 1f;
    [SerializeField] private bool invertDrag = false;

    private bool isDragging;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = GetComponent<Camera>();

        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouse();
#else
        HandleTouch();
#endif
    }

    private void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI())
                return;

            isDragging = true;
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            DragByScreenDelta(delta);
        }

        if (Input.GetMouseButtonUp(0))
            isDragging = false;
    }

    private void HandleTouch()
    {
        if (Input.touchCount == 0)
        {
            isDragging = false;
            return;
        }

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            if (IsPointerOverUI(touch.fingerId))
                return;

            isDragging = true;
        }
        else if (touch.phase == TouchPhase.Moved && isDragging)
        {
            DragByPixelDelta(touch.deltaPosition);
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            isDragging = false;
        }
    }

    private void DragByScreenDelta(Vector2 delta)
    {
        float unitsPerPixel = GetUnitsPerPixel();
        Vector3 move = new Vector3(delta.x, delta.y, 0f) * unitsPerPixel * dragSpeed;

        if (!invertDrag)
            move = -move;

        ApplyMove(move);
    }

    private void DragByPixelDelta(Vector2 delta)
    {
        float unitsPerPixel = GetUnitsPerPixel();
        Vector3 move = new Vector3(delta.x, delta.y, 0f) * unitsPerPixel * dragSpeed;

        if (!invertDrag)
            move = -move;

        ApplyMove(move);
    }

    private void ApplyMove(Vector3 move)
    {
        Vector3 oldPosition = targetCamera.transform.position;
        Vector3 targetPosition = oldPosition + move;

        if (!allowHorizontal)
            targetPosition.x = oldPosition.x;

        if (!allowVertical)
            targetPosition.y = oldPosition.y;

        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        targetPosition.z = oldPosition.z;
        targetCamera.transform.position = targetPosition;
    }

    private float GetUnitsPerPixel()
    {
        if (targetCamera.orthographic)
            return (targetCamera.orthographicSize * 2f) / Screen.height;

        return 0.01f;
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    private bool IsPointerOverUI(int fingerId)
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(fingerId);
    }
}