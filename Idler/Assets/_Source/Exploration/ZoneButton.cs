using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ZoneButton : MonoBehaviour
{
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
    if (_explorationExecutor != null && _zoneData != null)
    {
      _explorationExecutor.SelectZone(_zoneData);
    }
    else
    {
      Debug.LogError($"Cannot select zone! Executor: {_explorationExecutor}, ZoneData: {_zoneData}");
    }
  }
    
  public void UpdateVisual(bool isComplete, bool isUnlocked, bool isExploring)
  {
    if (_buttonImage != null)
    {
      if (isComplete)
        _buttonImage.color = Color.green;
      else if (isExploring)
        _buttonImage.color = Color.yellow;
      else
        _buttonImage.color = Color.white;
    }
        
    if (_buttonText != null)
    {
      string text = _zoneData != null ? _zoneData.zoneName : "Unknown";
      _buttonText.text = isComplete ? $"{text} ✓" : text;
    }
        
    if (_button != null)
    {
      _button.interactable = isUnlocked && !isComplete && !isExploring;
    }
  }
}