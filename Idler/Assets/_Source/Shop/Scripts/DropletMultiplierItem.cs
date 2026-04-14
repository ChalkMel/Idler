using UnityEngine;

[CreateAssetMenu(fileName = "NewDropletMultiplier", menuName = "Shop/Shop Item/Droplet Multiplier")]
public class DropletMultiplierItem : ShopItem
{
  [Header("Multiplier Settings")]
  public float multiplierValue = 2f;

  public override void ApplyEffect(ShopItem item, Credits credits, SpiritBuffManager spiritBuffManager)
  {
    base.ApplyEffect(item, credits, spiritBuffManager);
    isPurchased = true;
    credits.dropletsMulti += multiplierValue;
  }
}