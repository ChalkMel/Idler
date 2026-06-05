using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExplorationTimerUI : MonoBehaviour
{
  [SerializeField] private GameObject panel;
  [SerializeField] private Slider _timerSlider;
  [SerializeField] private TextMeshProUGUI _timerText;
  [SerializeField] private TextMeshProUGUI _timerMainScreenText;
    
  public void ShowTimer(float currentTime, float maxTime)
  {
    panel.SetActive(true);
    if (_timerSlider != null)
    {
      _timerSlider.gameObject.SetActive(true);
      _timerSlider.maxValue = maxTime;
      _timerSlider.value = maxTime - currentTime;
    }
        
    if (_timerText != null)
    {
      _timerText.text = $"{Mathf.Round(currentTime)} сек";
      _timerText.gameObject.SetActive(true);
    }
        
    if (_timerMainScreenText != null)
    {
      _timerMainScreenText.gameObject.SetActive(true);
      _timerMainScreenText.text = $"{Mathf.Round(currentTime)} сек";
    }
  }
    
  public void HideTimer()
  {
    panel.SetActive(false);
    if (_timerSlider != null)
      _timerSlider.gameObject.SetActive(false);
        
    if (_timerText != null)
      _timerText.gameObject.SetActive(false);
        
    if (_timerMainScreenText != null)
      _timerMainScreenText.gameObject.SetActive(false);
  }
    
  public void UpdateTimer(float currentTime, float maxTime)
  {
    if (_timerSlider != null)
      _timerSlider.value = maxTime - currentTime;
        
    if (_timerText != null)
      _timerText.text = $"{Mathf.Round(currentTime)} сек";
        
    if (_timerMainScreenText != null)
      _timerMainScreenText.text = $"{Mathf.Round(currentTime)} сек";
  }
}
