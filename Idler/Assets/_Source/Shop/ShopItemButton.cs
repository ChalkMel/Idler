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
    if (itemIcon != null && shopItem.icon != null)
    {
      itemIcon.sprite = shopItem.icon;
    }
        
    if (itemNameText != null)
    {
      itemNameText.text = shopItem.itemName;
    }
        
    if (itemDescriptionText != null)
    {
      itemDescriptionText.text = shopItem.description;
    }
        
    if (itemPriceText != null)
    {
      if (shopItem.isPurchased)
      {
        itemPriceText.text = "КУПЛЕНО";
        itemPriceText.color = Color.green;
      }
      else
      {
        itemPriceText.text = $"{shopItem.cost} капель";
        itemPriceText.color = Color.white;
      }
    }
    
    Image bgImage = GetComponent<Image>();
    if (bgImage != null)
    {
      bgImage.color = shopItem.isPurchased ? 
        new Color(0.3f, 0.6f, 0.3f) : 
        Color.white;
    }
  }
}