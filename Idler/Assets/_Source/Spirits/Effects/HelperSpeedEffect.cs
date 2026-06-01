namespace Effects
{
  using UnityEngine;

  [CreateAssetMenu(fileName = "HelperSpeedEffect", menuName = "Spirit/Effect/Helper Speed")]
  public class HelperSpeedEffect : SpiritEffect
  {
    [SerializeField] private float speedMultiplier = 2f;
    
    public override void Apply(Credits credits, SpiritBuffManager buffManager, float multiplier)
    {
      float effectiveMultiplier = speedMultiplier * multiplier;
      credits.helperCollectionInterval /= effectiveMultiplier;
      credits.DropletHelperCollectionInterval /= effectiveMultiplier;
    }
    
    public override void Remove(Credits credits, SpiritBuffManager buffManager, float multiplier)
    {
      float effectiveMultiplier = speedMultiplier * multiplier;
      credits.helperCollectionInterval *= effectiveMultiplier;
      credits.DropletHelperCollectionInterval *= effectiveMultiplier;
    }
  }
}