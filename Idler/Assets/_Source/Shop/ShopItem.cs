using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Shop Item")]
public class ShopItem : ScriptableObject
{
  [Header("Basic Info")]
  public string itemName;
  public Sprite icon;
  [TextArea(2, 4)] public string description;
    
  [Header("Purchase")]
  public int cost = 100;
  public bool isPurchased = false;
    
  [Header("Effect")]
  public ShopItemType itemType;
  public float effectValue = 1.0f;
  
  public void ApplyEffect()
  {
    // 
  }
}

public enum ShopItemType
{
  CauldronUpgrade,    
  BrewingSpeed,       
  IngredientBoost,    
  SpiritChance,       
  DropletMultiplier   
}