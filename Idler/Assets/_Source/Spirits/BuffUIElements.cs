using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffUIElement : MonoBehaviour
{
  [SerializeField] private Image buffIcon;
  [SerializeField] private TextMeshProUGUI buffNameText;
  [SerializeField] private TextMeshProUGUI buffTimerText;
  [SerializeField] private Slider timeSlider;
    
  private SpiritBuff _currentBuff;
    
  public void Setup(SpiritBuff buff)
  {
    _currentBuff = buff;
        
    if (buffIcon != null && buff.spirit != null && buff.spirit.icon != null)
    {
      buffIcon.sprite = buff.spirit.icon;
    }
    buffNameText.text = buff.buffName;
    
  }
    
  private void Update()
  {
    if (_currentBuff != null)
    {
      buffTimerText.text = Mathf.CeilToInt(_currentBuff.TimeLeft) + "with";
      timeSlider.value = _currentBuff.TimeLeft / _currentBuff.duration;
      
      if (!_currentBuff.IsActive)
      {
        Destroy(gameObject);
      }
    }
  }
}