using UnityEngine;

[CreateAssetMenu(fileName = "NewHelper", menuName = "Shop/Shop Item/Helper")]
public class HelperItem : ShopItem
{
  [Header("Helper Settings")]
  public int baseCost = 50;
  public int costMultiplier = 2;

  public override void ApplyEffect(ShopItem item, Credits credits, SpiritBuffManager spiritBuffManager)
  {
    base.ApplyEffect(item, credits, spiritBuffManager);
    isPurchased = false;
    cost *= costMultiplier;
    credits.HelperCount++;
    
  }
}