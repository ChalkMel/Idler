using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ZoneButton : MonoBehaviour
{
  public System.Action onZoneClicked;
  
  [SerializeField] private ZoneData _zoneData;
  [SerializeField] private Image _buttonImage;
  [SerializeField] private TextMeshProUGUI _buttonText;
    
  // Добавляем ссылку на ExplorationExecutor
  [SerializeField] private ExplorationExecutor _explorationExecutor;
    
  private Button _button;
    
  public ZoneData ZoneData => _zoneData;
    
  private void Start()
  {
    _button = GetComponent<Button>();
        
    // Если ссылка не назначена в инспекторе - ищем автоматически
    if (_explorationExecutor == null)
    {
      _explorationExecutor = FindFirstObjectByType<ExplorationExecutor>();
    }
        
    if (_button != null)
    {
      _button.onClick.AddListener(OnButtonClick);
    }
  }
    
  private void OnButtonClick()
  {
    onZoneClicked?.Invoke();
    
    if (_explorationExecutor != null && _zoneData != null)
    {
      _explorationExecutor.SelectZone(_zoneData);
    }
    else
    {
      Debug.LogError($"Cannot select zone! Executor: {_explorationExecutor}, ZoneData: {_zoneData}");
    }
  }
}