using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarCollectible : MonoBehaviour
{



    [SerializeField] private GameSceneController gameSceneController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (gameSceneController != null)
            gameSceneController.OnLevelStarCollected();

        Destroy(gameObject);
    }
}

