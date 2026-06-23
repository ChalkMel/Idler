using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "NewZone", menuName = "Explore/Zone Data")]
public class ZoneData : ScriptableObject
{
  public string ZoneName { get; private set; }
  public string ZoneDescription { get; private set; }
  public Sprite ZoneIcon { get; private set; }
  public float ExplorationTime {get ; private set;}
  
  public List<IngredientData> ingredients;

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
      if (spirit != null && !playerSpirits.UnlockedSpirits.Contains(spirit))
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
      if (spirit != null && !playerSpirits.UnlockedSpirits.Contains(spirit))
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