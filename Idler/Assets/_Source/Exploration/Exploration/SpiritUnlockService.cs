using UnityEngine;
using UnityEngine.Serialization;

public class SpiritUnlockService : MonoBehaviour
{
  [SerializeField] private SpiritCollection spiritCollection;
    
  public event System.Action<SpiritData> OnSpiritUnlocked;
    
  public bool TryUnlockSpirit(SpiritData spirit)
  {
    if (spirit == null || spiritCollection == null) return false;
        
    if (spiritCollection.UnlockedSpirits.Contains(spirit))
    {
      return false;
    }
        
    if (spiritCollection.UnlockSpirit(spirit))
    {
      OnSpiritUnlocked?.Invoke(spirit);
      return true;
    }
        
    return false;
  }
    
  public bool TryUnlockRandomSpiritFromZone(ZoneData zone)
  {
    if (zone == null) return false;
    
    System.Collections.Generic.List<SpiritData> lockedSpirits = new System.Collections.Generic.List<SpiritData>();
    foreach (var spirit in zone.availableSpirits)
    {
      if (spirit != null && !spiritCollection.UnlockedSpirits.Contains(spirit))
      {
        lockedSpirits.Add(spirit);
      }
    }
        
    if (lockedSpirits.Count == 0)
    {
      return false;
    }
        
    int randomIndex = Random.Range(0, lockedSpirits.Count);
    return TryUnlockSpirit(lockedSpirits[randomIndex]);
  }
}