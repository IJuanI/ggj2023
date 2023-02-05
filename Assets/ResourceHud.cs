using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceHud : MonoBehaviour
{

    public Image iconSkin;
    public TextMeshProUGUI amountText;

    public void SetIcon(Sprite icon) {
        iconSkin.sprite = icon;
    }

    public void SetAmount(int amount) {
        amountText.text = amount.ToString();
    }

}
