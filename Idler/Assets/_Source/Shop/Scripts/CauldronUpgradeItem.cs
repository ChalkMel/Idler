using UnityEngine;

[CreateAssetMenu(fileName = "NewCauldronUpgrade", menuName = "Shop/Shop Item/Cauldron Upgrade")]
public class CauldronUpgradeItem : ShopItem
{
  [Header("Cauldron Settings")]
  public float brewingTimeReduction = 2f;
  public TeaCollection teaCollection;

  public override void ApplyEffect(ShopItem item, Credits credits, SpiritBuffManager spiritBuffManager)
  {
    base.ApplyEffect(item, credits, spiritBuffManager);
    isPurchased = true;
    
    foreach (TeaData tea in teaCollection.allTeas)
    {
      tea.brewingTime /= brewingTimeReduction;
    }
  }
}