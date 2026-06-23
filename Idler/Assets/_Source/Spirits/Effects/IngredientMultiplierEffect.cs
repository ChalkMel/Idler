using UnityEngine;

namespace Effects
{
  [CreateAssetMenu(fileName = "IngredientMultiplier", menuName = "Spirit/Effect/Ingredient Multiplier")]
  public class IngredientMultiplierEffect : SpiritEffect
  {
    public override void Apply(Credits credits, SpiritBuffManager buffManager, float multiplier)
    {
      credits.BerriesMulti += multiplier;
      credits.FlowersMulti += multiplier;
    }

    public override void Remove(Credits credits, SpiritBuffManager buffManager, float multiplier)
    {
      credits.BerriesMulti -= multiplier;
      credits.FlowersMulti -= multiplier;
    }
  }
}