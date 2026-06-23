using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class ZoneButton : MonoBehaviour
{
  [SerializeField] private ZoneData zoneData;
  [SerializeField] private Image buttonImage;
  [SerializeField] private TextMeshProUGUI buttonText;
  [SerializeField] private ExplorationExecutor explorationExecutor;
    
  private Button _button;
    
  public ZoneData ZoneData => zoneData;
    
  private void Start()
  {
    _button = GetComponent<Button>();
    _button.onClick.AddListener(OnButtonClick);
  }
    
  private void OnButtonClick()
  {
    if (explorationExecutor != null && zoneData != null)
    {
      explorationExecutor.SelectZone(zoneData);
    }
    else
    {
      Debug.LogError($"Cannot select zone! Executor: {explorationExecutor}, ZoneData: {zoneData}");
    }
  }
}