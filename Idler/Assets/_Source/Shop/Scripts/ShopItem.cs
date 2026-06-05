using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Shop Item")]
public abstract class ShopItem : ScriptableObject
{
  [Header("Basic Info")]
  public string itemName;
  public Sprite icon;
  [TextArea(2, 4)] public string description;
    
  [Header("Purchase")]
  public int cost;
  public bool isPurchased = false;
    
  [Header("Progression")]
  public int baseCost = 50;
  public int costMultiplier = 2;
  public int boughtCount = 0;

  public virtual void ApplyEffect(ShopItem item, Credits credits, SpiritBuffManager spiritBuffManager)
  {
  }
    
  public virtual void UpdateCost()
  {
    cost = baseCost * Mathf.RoundToInt(Mathf.Pow(costMultiplier, boughtCount));
  }
    
  public virtual void ResetToBase()
  {
    boughtCount = 0;
    isPurchased = false;
    UpdateCost();
  }
}