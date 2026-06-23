using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpiritCollection", menuName = "Tea/Spirit Collection")]
public class SpiritCollection : ScriptableObject
{
  public List<SpiritData> AllSpirits { get; private set; } = new List<SpiritData>();
  public List<SpiritData> UnlockedSpirits { get; private set; } = new List<SpiritData>();
  public List<SpiritData> AvailableSpirits { get; private set; } = new List<SpiritData>();
  
  public bool UnlockSpirit(SpiritData spirit)
  {
    if (UnlockedSpirits.Contains(spirit))
      return false;
            
    UnlockedSpirits.Add(spirit);
    AvailableSpirits.Add(spirit);
    spirit.isUnlocked = true;
    return true;
  }

  public void MakeSpiritAvailable(SpiritData spirit, bool isAvailable)
  {
    if(isAvailable)
      AvailableSpirits.Add(spirit);
    else
      AvailableSpirits.Remove(spirit);
  }
}