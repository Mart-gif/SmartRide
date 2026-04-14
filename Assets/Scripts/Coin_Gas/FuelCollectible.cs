using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelCollectible : MonoBehaviour
{
    [SerializeField] private int fuelAmount = 3;
    [SerializeField] private GameObject collectEffect;

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
            return;

        if (!other.CompareTag("Player"))
            return;

        collected = true;

        GameSceneController controller = FindObjectOfType<GameSceneController>();
        if (controller != null)
        {
            controller.AddFuel(fuelAmount);
            controller.PlayFuelCollectSfx();
        }

        if (collectEffect != null)
            Instantiate(collectEffect, transform.position, Quaternion.identity);

        gameObject.SetActive(false);
    }
}
