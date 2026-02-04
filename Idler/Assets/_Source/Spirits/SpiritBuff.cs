using UnityEngine;

[System.Serializable]
public class SpiritBuff
{
  public string buffName;
  public float multiplier;
  public float duration;
  public float endTime;
  public SpiritData spirit;
    
  public bool IsActive => Time.time < endTime;
  public float TimeLeft => Mathf.Max(0, endTime - Time.time);
    
  public string GetInfoText()
  {
    if (spirit == null) return "Нет буста";
    return $"{spirit.spiritName}\n{spirit.buffName}\nx{multiplier:F1} на {duration}с";
  }
}