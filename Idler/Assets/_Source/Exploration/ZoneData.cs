using UnityEngine;

[CreateAssetMenu(fileName = "NewZone", menuName = "Game/Zones/Zone Data")]
public class ZoneData : ScriptableObject
{
  public string zoneName;
  public string zoneDescription;
  public Sprite zoneIcon;
  public float explorationTime = 10f;
}