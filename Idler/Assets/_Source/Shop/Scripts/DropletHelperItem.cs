using UnityEngine;

[CreateAssetMenu(fileName = "NewHelper", menuName = "Shop/Shop Item/DropletHelper")]
public class HelperDropletItem : ShopItem
{
  [Header("Helper Settings")]
  public int baseCost = 50;
  public int costMultiplier = 2;
  private int bought = 0;

  public override void ApplyEffect(ShopItem item, Credits credits, SpiritBuffManager spiritBuffManager)
  {
    base.ApplyEffect(item, credits, spiritBuffManager);
    bought++;
    isPurchased = false;
    cost *= costMultiplier;
    credits.DropletHelperCount++;
  }
}