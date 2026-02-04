using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpiritCollection", menuName = "Tea/Spirit Collection")]
public class SpiritCollection : ScriptableObject
{
  public List<SpiritData> allSpirits = new List<SpiritData>();
  public List<SpiritData> unlockedSpirits = new List<SpiritData>();
  
  public bool UnlockSpirit(SpiritData spirit)
  {
    if (unlockedSpirits.Contains(spirit))
      return false;
            
    unlockedSpirits.Add(spirit);
    spirit.isUnlocked = true;
    return true;
  }
}