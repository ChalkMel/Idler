using UnityEngine;

public class SpiritUnlockService : MonoBehaviour
{
  [SerializeField] private SpiritCollection _spiritCollection;
    
  public event System.Action<SpiritData> OnSpiritUnlocked;
    
  public bool TryUnlockSpirit(SpiritData spirit)
  {
    if (spirit == null || _spiritCollection == null) return false;
        
    if (_spiritCollection.unlockedSpirits.Contains(spirit))
    {
      Debug.Log($"Spirit {spirit.spiritName} already unlocked!");
      return false;
    }
        
    if (_spiritCollection.UnlockSpirit(spirit))
    {
      Debug.Log($"Unlocked new spirit: {spirit.spiritName}");
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
      if (spirit != null && !_spiritCollection.unlockedSpirits.Contains(spirit))
      {
        lockedSpirits.Add(spirit);
      }
    }
        
    if (lockedSpirits.Count == 0)
    {
      Debug.Log($"No locked spirits in zone: {zone.zoneName}");
      return false;
    }
        
    int randomIndex = Random.Range(0, lockedSpirits.Count);
    return TryUnlockSpirit(lockedSpirits[randomIndex]);
  }
}