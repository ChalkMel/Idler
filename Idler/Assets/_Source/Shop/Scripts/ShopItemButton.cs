using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class ShopItemButton : MonoBehaviour
{
  [Header("References")]
  public ShopItem ShopItem;
 
  [Header("UI Elements")]
  [SerializeField] public Image ItemIcon;
  [SerializeField] public TextMeshProUGUI ItemNameText;
  [SerializeField] public TextMeshProUGUI ItemDescriptionText;
  [SerializeField] public TextMeshProUGUI ItemPriceText;
    
  private void Start()
  {
    if (ShopItem != null)
    {
      SetupUI();
    }
  }
    
  private void OnEnable()
  {
    if (ShopItem != null)
    {
      SetupUI();
    }
  }
    
  private void SetupUI()
  {
    ItemIcon.sprite = ShopItem.Icon;
    ItemNameText.text = ShopItem.ItemName;
    ItemDescriptionText.text = ShopItem.Description;
  }
}