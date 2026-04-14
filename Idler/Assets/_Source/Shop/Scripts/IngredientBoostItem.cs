using UnityEngine;

[CreateAssetMenu(fileName = "NewIngredientBoost", menuName = "Shop/Shop Item/Ingredient Boost")]
public class IngredientBoostItem : ShopItem
{
  [Header("Boost Settings")]
  public float boostValue = 0.5f;

  public override void ApplyEffect(ShopItem item, Credits credits, SpiritBuffManager spiritBuffManager)
  {
    base.ApplyEffect(item, credits, spiritBuffManager);
    isPurchased = true;
    credits.leavesMulti += boostValue;
    credits.berriesMulti += boostValue;
    credits.flowersMulti += boostValue;
  }
}