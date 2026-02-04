using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewZone", menuName = "Game/Zones/Zone Data")]
public class ZoneData : ScriptableObject
{
  public string zoneName;
  public string zoneDescription;
  public Sprite zoneIcon;
  public float explorationTime = 10f;
    
  [Header("Spirits in this zone")]
  public List<SpiritData> availableSpirits = new List<SpiritData>();
    
  [Header("Zone State")]
  public bool isUnlocked = true;
  
  public bool AreAllSpiritsFound(SpiritCollection playerSpirits)
  {
    if (playerSpirits == null || availableSpirits.Count == 0)
      return false;
            
    foreach (var spirit in availableSpirits)
    {
      if (spirit != null && !playerSpirits.unlockedSpirits.Contains(spirit))
        return false;
    }
    return true;
  }
  
  public List<SpiritData> GetUnfoundSpirits(SpiritCollection playerSpirits)
  {
    List<SpiritData> unfoundSpirits = new List<SpiritData>();
        
    if (playerSpirits == null) 
      return availableSpirits;
            
    foreach (var spirit in availableSpirits)
    {
      if (spirit != null && !playerSpirits.unlockedSpirits.Contains(spirit))
      {
        unfoundSpirits.Add(spirit);
      }
    }
    return unfoundSpirits;
  }
  
  public SpiritData GetRandomUnfoundSpirit(SpiritCollection playerSpirits)
  {
    List<SpiritData> unfoundSpirits = GetUnfoundSpirits(playerSpirits);
        
    if (unfoundSpirits.Count == 0)
      return null;
            
    int randomIndex = Random.Range(0, unfoundSpirits.Count);
    return unfoundSpirits[randomIndex];
  }
}