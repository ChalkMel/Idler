using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpiritFoundPopupUI : MonoBehaviour
{
  [SerializeField] private GameObject _popupPanel;
  [SerializeField] private Image _spiritIcon;
  [SerializeField] private TextMeshProUGUI _spiritNameText;
  [SerializeField] private TextMeshProUGUI _spiritDescriptionText;
  [SerializeField] private Button _closeButton;
  [SerializeField] private float _autoCloseDelay = 5f;
    
  private Coroutine _autoCloseCoroutine;
    
  private void Start()
  {
    if (_closeButton != null)
      _closeButton.onClick.AddListener(ClosePopup);
        
    _popupPanel.SetActive(false);
  }
    
  public void ShowSpiritFound(SpiritData spirit)
  {
    if (_popupPanel == null || spirit == null) return;
        
    if (_spiritIcon != null)
    {
      _spiritIcon.sprite = spirit.icon;
      _spiritIcon.preserveAspect = true;
    }
        
    if (_spiritNameText != null)
      _spiritNameText.text = $"Found spirit: {spirit.spiritName}";
        
    if (_spiritDescriptionText != null)
      _spiritDescriptionText.text = $"{spirit.description}\n\nBoost: {spirit.buffName}\n{spirit.buffDescription}";
        
    _popupPanel.SetActive(true);
        
    if (_autoCloseCoroutine != null)
      StopCoroutine(_autoCloseCoroutine);
    _autoCloseCoroutine = StartCoroutine(AutoCloseCoroutine());
  }
    
  private IEnumerator AutoCloseCoroutine()
  {
    yield return new WaitForSeconds(_autoCloseDelay);
    ClosePopup();
  }
    
  private void ClosePopup()
  {
    _popupPanel.SetActive(false);
        
    if (_autoCloseCoroutine != null)
    {
      StopCoroutine(_autoCloseCoroutine);
      _autoCloseCoroutine = null;
    }
  }
}