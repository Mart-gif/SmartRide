using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    [SerializeField] private GameSceneController gameSceneController;
    [SerializeField] private PlayerPathController player;
    [SerializeField] private PathNode finishNode;

    private bool completed;

    private void Update()
    {
        if (completed)
            return;

        if (gameSceneController == null || player == null || finishNode == null)
            return;

        if (player.CurrentNode == finishNode && !player.IsMoving())
        {
            completed = true;
            gameSceneController.CompleteLevel();
        }
    }
}