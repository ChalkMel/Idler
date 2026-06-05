using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemButton : MonoBehaviour
{
  [Header("References")]
  public ShopItem shopItem;
    
  [Header("UI Elements")]
  [SerializeField] public Image itemIcon;
  [SerializeField] public TextMeshProUGUI itemNameText;
  [SerializeField] public TextMeshProUGUI itemDescriptionText;
  [SerializeField] public TextMeshProUGUI itemPriceText;
    
  private void Start()
  {
    if (shopItem != null)
    {
      SetupUI();
    }
  }
    
  private void OnEnable()
  {
    if (shopItem != null)
    {
      SetupUI();
    }
  }
    
  private void SetupUI()
  {
    itemIcon.sprite = shopItem.icon;
    itemNameText.text = shopItem.itemName;
    itemDescriptionText.text = shopItem.description;
  }
}