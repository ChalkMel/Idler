using UnityEngine;

namespace Effects
{
  public abstract class SpiritEffect : ScriptableObject
  {
    public abstract void Apply(Credits credits, SpiritBuffManager buffManager, float multiplier);
    public abstract void Remove(Credits credits, SpiritBuffManager buffManager, float multiplier);
  }
}