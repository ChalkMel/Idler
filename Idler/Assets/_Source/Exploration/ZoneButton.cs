using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ZoneButton : MonoBehaviour
{
  [SerializeField] public int zoneIndex;
  [SerializeField] private Exploration explorationManager;
    
  [Header("UI Elements")]
  [SerializeField] private Image zoneIcon;
  [SerializeField] private TextMeshProUGUI zoneNameText;
  [SerializeField] private TextMeshProUGUI progressText;
    
  private Button _button;
    
  private void Start()
  {
    _button = GetComponent<Button>();
    _button.onClick.AddListener(OnClick);
        
    // Настраиваем UI кнопки
    UpdateButtonUI();
  }
    
  private void OnClick()
  {
    if (explorationManager != null)
    {
      explorationManager.SelectZone(zoneIndex);
    }
  }
    
  public void UpdateButtonUI()
  {
    // Можно обновлять внешний вид кнопки
    // Например, показывать прогресс по духам
  }
}