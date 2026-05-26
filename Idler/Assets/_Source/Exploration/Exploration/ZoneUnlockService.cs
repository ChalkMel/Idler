using System.Collections.Generic;
using UnityEngine;

public class ZoneUnlockService : MonoBehaviour
{
  [SerializeField] private SpiritCollection _spiritCollection;
    
  public bool IsZoneComplete(ZoneData zone)
  {
    if (zone == null || _spiritCollection == null) return false;
        
    foreach (var spirit in zone.availableSpirits)
    {
      if (spirit != null && !_spiritCollection.unlockedSpirits.Contains(spirit))
        return false;
    }
    return true;
  }
    
  public List<SpiritData> GetUnfoundSpirits(ZoneData zone)
  {
    List<SpiritData> unfound = new List<SpiritData>();
    if (zone == null || _spiritCollection == null) return unfound;
        
    foreach (var spirit in zone.availableSpirits)
    {
      if (spirit != null && !_spiritCollection.unlockedSpirits.Contains(spirit))
      {
        unfound.Add(spirit);
      }
    }
    return unfound;
  }
    
  public SpiritData GetRandomUnfoundSpirit(ZoneData zone)
  {
    List<SpiritData> unfound = GetUnfoundSpirits(zone);
    if (unfound.Count == 0) return null;
        
    int randomIndex = Random.Range(0, unfound.Count);
    return unfound[randomIndex];
  }
    
  public string GetZoneProgressText(ZoneData zone)
  {
    if (zone == null) return "Unknown zone";
        
    List<SpiritData> unfound = GetUnfoundSpirits(zone);
    int foundCount = zone.availableSpirits.Count - unfound.Count;
    return $"Spirits: {foundCount}/{zone.availableSpirits.Count} found";
  }
}