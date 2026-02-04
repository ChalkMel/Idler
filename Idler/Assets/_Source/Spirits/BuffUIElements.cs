using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffUIElement : MonoBehaviour
{
  [SerializeField] private Image buffIcon;
  [SerializeField] private TextMeshProUGUI buffNameText;
  [SerializeField] private TextMeshProUGUI buffTimerText;
  [SerializeField] private Slider timeSlider;
    
  private SpiritBuff currentBuff;
    
  public void Setup(SpiritBuff buff)
  {
    currentBuff = buff;
        
    if (buffIcon != null && buff.spirit != null && buff.spirit.icon != null)
    {
      buffIcon.sprite = buff.spirit.icon;
    }
        
    if (buffNameText != null)
    {
      buffNameText.text = buff.buffName;
    }
  }
    
  private void Update()
  {
    if (currentBuff != null)
    {
      if (buffTimerText != null)
      {
        buffTimerText.text = Mathf.CeilToInt(currentBuff.TimeLeft) + "с";
      }
            
      if (timeSlider != null)
      {
        timeSlider.value = currentBuff.TimeLeft / currentBuff.duration;
      }

      if (!currentBuff.IsActive)
      {
        Destroy(gameObject);
      }
    }
  }
}