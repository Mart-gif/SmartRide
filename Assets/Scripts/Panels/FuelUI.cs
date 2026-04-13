using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelUI : MonoBehaviour
{
    [Header("Digit Images")]
    [SerializeField] private Image tensDigitImage;
    [SerializeField] private Image onesDigitImage;

    [Header("Digit Sprites")]
    [SerializeField] private Sprite[] digitSprites; // 0..9

    public void SetFuel(int fuelValue)
    {
        fuelValue = Mathf.Max(0, fuelValue);

        int tens = fuelValue / 10;
        int ones = fuelValue % 10;

        if (tensDigitImage != null)
        {
            if (fuelValue >= 10)
            {
                tensDigitImage.gameObject.SetActive(true);
                tensDigitImage.sprite = GetDigitSprite(tens);
            }
            else
            {
                tensDigitImage.gameObject.SetActive(false);
            }
        }

        if (onesDigitImage != null)
        {
            onesDigitImage.sprite = GetDigitSprite(ones);
        }
    }

    private Sprite GetDigitSprite(int digit)
    {
        if (digitSprites == null || digitSprites.Length < 10)
            return null;

        if (digit < 0 || digit > 9)
            return null;

        return digitSprites[digit];
    }
}