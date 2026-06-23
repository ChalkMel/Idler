using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Shop Item")]
public abstract class ShopItem : ScriptableObject
{
  [Header("Basic Info")]
  public string ItemName {get; private set;}
  public Sprite Icon {get; private set;} 
  [TextArea(2, 4)] public string Description {get; private set;}
    
  [Header("Purchase")]
  public int Cost {get; private set;}

  public bool isPurchased;
    
  [Header("Progression")]
  public int BaseCost = 50;
  public int CostMultiplier = 2;
  public int BoughtCount = 0;

  public virtual void ApplyEffect(ShopItem item, Credits credits, SpiritBuffManager spiritBuffManager)
  {
  }
    
  public virtual void UpdateCost()
  {
    Cost = BaseCost * Mathf.RoundToInt(Mathf.Pow(CostMultiplier, BoughtCount));
  }
    
  public virtual void ResetToBase()
  {
    BoughtCount = 0;
    isPurchased = false;
    UpdateCost();
  }
}