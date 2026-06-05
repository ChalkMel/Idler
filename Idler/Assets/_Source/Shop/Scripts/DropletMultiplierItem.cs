using UnityEngine;

[CreateAssetMenu(fileName = "NewDropletMultiplier", menuName = "Shop/Shop Item/Droplet Multiplier")]
public class DropletMultiplierItem : ShopItem
{
  [Header("Multiplier Settings")]
  public float multiplierValue = 2f;
  public float totalMultiplierValue = 0f;

  public override void ApplyEffect(ShopItem item, Credits credits, SpiritBuffManager spiritBuffManager)
  {
    base.ApplyEffect(item, credits, spiritBuffManager);
        
    boughtCount++;
    totalMultiplierValue += multiplierValue;
    isPurchased = false;
    UpdateCost();
        
    credits.dropletsMulti += multiplierValue;
  }
    
  public override void ResetToBase()
  {
    base.ResetToBase();
    totalMultiplierValue = 0f;
  }
}