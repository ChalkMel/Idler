using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewIngredientBoost", menuName = "Shop/Shop Item/Ingredient Boost")]
public class IngredientBoostItem : ShopItem
{
  [Header("Boost Settings")] 
  [SerializeField] private float _boostValue = 0.5f;

  public override void ApplyEffect(ShopItem item, Credits credits, SpiritBuffManager spiritBuffManager)
  {
    base.ApplyEffect(item, credits, spiritBuffManager);
    isPurchased = true;
    credits.LeavesMulti += _boostValue;
    credits.BerriesMulti += _boostValue;
    credits.FlowersMulti += _boostValue;
  }
}