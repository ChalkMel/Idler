using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class SpiritFoundPopupUI : MonoBehaviour
{
  [SerializeField] private GameObject popupPanel;
  [SerializeField] private Image spiritIcon;
  [SerializeField] private TextMeshProUGUI spiritNameText;
  [SerializeField] private TextMeshProUGUI spiritDescriptionText;
  [SerializeField] private Button closeButton;
  [SerializeField] private float autoCloseDelay = 5f;
    
  private Coroutine _autoCloseCoroutine;
    
  private void Start()
  {
    if (closeButton != null)
      closeButton.onClick.AddListener(ClosePopup);
        
    popupPanel.SetActive(false);
  }
    
  public void ShowSpiritFound(SpiritData spirit)
  {
    if (popupPanel == null || spirit == null) return;
        
    if (spiritIcon != null)
    {
      spiritIcon.sprite = spirit.Icon;
      spiritIcon.preserveAspect = true;
    }
        
    if (spiritNameText != null)
      spiritNameText.text = $"Найден дух: {spirit.SpiritName}";
        
    if (spiritDescriptionText != null)
      spiritDescriptionText.text = $"{spirit.Description}\n\nБуст: {spirit.BuffName}\n{spirit.BuffDescription}";
        
    popupPanel.SetActive(true);
        
    if (_autoCloseCoroutine != null)
      StopCoroutine(_autoCloseCoroutine);
    _autoCloseCoroutine = StartCoroutine(AutoCloseCoroutine());
  }
    
  private IEnumerator AutoCloseCoroutine()
  {
    yield return new WaitForSeconds(autoCloseDelay);
    ClosePopup();
  }
    
  private void ClosePopup()
  {
    popupPanel.SetActive(false);
        
    if (_autoCloseCoroutine != null)
    {
      StopCoroutine(_autoCloseCoroutine);
      _autoCloseCoroutine = null;
    }
  }
}