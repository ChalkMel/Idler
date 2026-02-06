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
    if (shopItem.isPurchased)
    {
      itemPriceText.text = "Bought";
      itemPriceText.color = Color.green;
    }
    else
    {
      itemPriceText.text = $"{shopItem.cost} droplets";
      itemPriceText.color = Color.white;
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