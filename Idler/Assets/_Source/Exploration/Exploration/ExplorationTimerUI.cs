using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class ExplorationTimerUI : MonoBehaviour
{
  [SerializeField] private GameObject panel;
  [SerializeField] private Slider timerSlider;
  [SerializeField] private TextMeshProUGUI timerText;
  [SerializeField] private TextMeshProUGUI timerMainScreenText;
    
  public void ShowTimer(float currentTime, float maxTime)
  {
    panel.SetActive(true);
    if (timerSlider != null)
    {
      timerSlider.gameObject.SetActive(true);
      timerSlider.maxValue = maxTime;
      timerSlider.value = maxTime - currentTime;
    }
        
    if (timerText != null)
    {
      timerText.text = $"{Mathf.Round(currentTime)} сек";
      timerText.gameObject.SetActive(true);
    }
        
    if (timerMainScreenText != null)
    {
      timerMainScreenText.gameObject.SetActive(true);
      timerMainScreenText.text = $"{Mathf.Round(currentTime)} сек";
    }
  }
    
  public void HideTimer()
  {
    panel.SetActive(false);
    if (timerSlider != null)
      timerSlider.gameObject.SetActive(false);
        
    if (timerText != null)
      timerText.gameObject.SetActive(false);
        
    if (timerMainScreenText != null)
      timerMainScreenText.gameObject.SetActive(false);
  }
    
  public void UpdateTimer(float currentTime, float maxTime)
  {
    if (timerSlider != null)
      timerSlider.value = maxTime - currentTime;
        
    if (timerText != null)
      timerText.text = $"{Mathf.Round(currentTime)} сек";
        
    if (timerMainScreenText != null)
      timerMainScreenText.text = $"{Mathf.Round(currentTime)} сек";
  }
}
