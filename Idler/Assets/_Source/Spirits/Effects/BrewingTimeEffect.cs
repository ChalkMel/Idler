
using Effects;
using UnityEngine;

[CreateAssetMenu(fileName = "BrewingTimeEffect", menuName = "Spirit/Effect/Brewing Time")]
public class BrewingTimeEffect : SpiritEffect
{
  [SerializeField] private bool useDivision = true; // true: деление, false: вычитание
    
  public override void Apply(Credits credits, SpiritBuffManager buffManager, float multiplier)
  {
    var teaMaker = FindFirstObjectByType<TeaBrewingController>();
    if (teaMaker == null) return;
        
    foreach (var tea in teaMaker.allTeas)
    {
      if (useDivision)
        tea.brewingTime /= multiplier;
      else
        tea.brewingTime -= multiplier;
    }
  }

  public override void Remove(Credits credits, SpiritBuffManager buffManager, float multiplier)
  {
    var teaMaker = FindFirstObjectByType<TeaBrewingController>();
    if (teaMaker == null) return;
        
    foreach (var tea in teaMaker.allTeas)
    {
      if (useDivision)
        tea.brewingTime *= multiplier;
      else
        tea.brewingTime += multiplier;
    }
  }
}