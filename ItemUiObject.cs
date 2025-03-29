using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI label;

    public void Setup(string foodName)
    {
        label.text = foodName;
        icon.sprite = Resources.Load<Sprite>($"FoodIcons/{foodName}");
    }

    public void OnClick() => GameManager.Instance.SpawnItem(label.text);
}
