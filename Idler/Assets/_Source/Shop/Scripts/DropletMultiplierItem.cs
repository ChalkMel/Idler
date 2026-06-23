using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewDropletMultiplier", menuName = "Shop/Shop Item/Droplet Multiplier")]
public class DropletMultiplierItem : ShopItem
{
  [Header("Multiplier Settings")]
  [SerializeField] private float multiplierValue = 2f;
  public float TotalMultiplierValue;

  public override void ApplyEffect(ShopItem item, Credits credits, SpiritBuffManager spiritBuffManager)
  {
    base.ApplyEffect(item, credits, spiritBuffManager);
        
    BoughtCount++;
    TotalMultiplierValue += multiplierValue;
    isPurchased = false;
    UpdateCost();
        
    credits.DropletsMulti += multiplierValue;
  }
    
  public override void ResetToBase()
  {
    base.ResetToBase();
    TotalMultiplierValue = 0f;
  }
}