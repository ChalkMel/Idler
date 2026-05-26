using UnityEngine;

public class Exploration : MonoBehaviour
{
  [SerializeField] private ExplorationExecutor _executor;
  [SerializeField] private ZoneSelectionUI _zoneSelectionUI;
    
  public void SelectZone(int zoneIndex)
  {
    // Получаем ZoneData по индексу
    // Этот метод нужен для совместимости со старым кодом
    Debug.Log($"Select zone index: {zoneIndex}");
  }
}