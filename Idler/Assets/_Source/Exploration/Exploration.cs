using UnityEngine;

public class Exploration : MonoBehaviour
{
  [SerializeField] private ExplorationExecutor _executor;
  [SerializeField] private ZoneSelectionUI _zoneSelectionUI;
    
  public void SelectZone(int zoneIndex)
  {
    Debug.Log($"Select zone index: {zoneIndex}");
  }
}