using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileMoveButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private PlayerController player;
    [SerializeField] private PlayerController.MoveDirection direction;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (player == null)
            return;

        player.Move(direction);
    }
}
