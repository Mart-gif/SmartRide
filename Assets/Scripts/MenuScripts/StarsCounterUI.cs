using UnityEngine;
using UnityEngine.UI;

public class StarsCounterUI : MonoBehaviour
{
    [SerializeField] private Image[] digits;
    [SerializeField] private Sprite[] numberSprites;

    public void SetNumber(int value)
    {
        string number = value.ToString();

        for (int i = 0; i < digits.Length; i++)
        {
            digits[i].gameObject.SetActive(false);
        }

        int digitIndex = digits.Length - 1;

        for (int i = number.Length - 1; i >= 0; i--)
        {
            int num = number[i] - '0';

            digits[digitIndex].sprite = numberSprites[num];
            digits[digitIndex].gameObject.SetActive(true);

            digitIndex--;

            if (digitIndex < 0)
                break;
        }
    }
}   