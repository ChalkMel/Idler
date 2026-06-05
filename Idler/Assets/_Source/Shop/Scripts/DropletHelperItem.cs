using UnityEngine;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDropletHelper", menuName = "Shop/Shop Item/Droplet Helper")]
public class HelperDropletItem : ShopItem
{
  public override void ApplyEffect(ShopItem item, Credits credits, SpiritBuffManager spiritBuffManager)
  {
    base.ApplyEffect(item, credits, spiritBuffManager);
        
    boughtCount++;
    isPurchased = false;
    UpdateCost();
        
    credits.DropletHelperCount++;
  }
}