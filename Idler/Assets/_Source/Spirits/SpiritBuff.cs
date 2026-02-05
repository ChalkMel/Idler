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
}