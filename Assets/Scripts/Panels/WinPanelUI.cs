using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinPanelUI : MonoBehaviour
{
    [Header("Star Images")]
    [SerializeField] private Image star1Image;
    [SerializeField] private Image star2Image;
    [SerializeField] private Image star3Image;

    [Header("Star Sprites")]
    [SerializeField] private Sprite starOnSprite;
    [SerializeField] private Sprite starOffSprite;

    public void ShowStars(int starCount)
    {
        SetStar(star1Image, starCount >= 1);
        SetStar(star2Image, starCount >= 2);
        SetStar(star3Image, starCount >= 3);
    }

    private void SetStar(Image targetImage, bool isOn)
    {
        if (targetImage == null)
            return;

        targetImage.sprite = isOn ? starOnSprite : starOffSprite;
    }
}
