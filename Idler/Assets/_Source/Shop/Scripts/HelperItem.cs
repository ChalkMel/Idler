using UnityEngine;

[CreateAssetMenu(fileName = "NewHelper", menuName = "Shop/Shop Item/Helper")]
public class HelperItem : ShopItem
{
  public override void ApplyEffect(ShopItem item, Credits credits, SpiritBuffManager spiritBuffManager)
  {
    base.ApplyEffect(item, credits, spiritBuffManager);
        
    boughtCount++;
    isPurchased = false;
    UpdateCost();
        
    credits.HelperCount++;
  }
}