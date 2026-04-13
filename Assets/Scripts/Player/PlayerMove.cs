using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMove : MonoBehaviour
{
    [SerializeField] private PlayerPathController player;

    public void MoveUp()
    {
        if (player == null) return;
        player.Move(MoveDirection.Up);
    }

    public void MoveDown()
    {
        if (player == null) return;
        player.Move(MoveDirection.Down);
    }

    public void MoveLeft()
    {
        if (player == null) return;
        player.Move(MoveDirection.Left);
    }

    public void MoveRight()
    {
        if (player == null) return;
        player.Move(MoveDirection.Right);
    }
}
