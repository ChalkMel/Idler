using UnityEngine;

[System.Serializable]
public class SpiritBuff
{
  public string buffName;
  public float multiplier;
  public float duration; // в секундах
  public float endTime; // время окончания действия
  public SpiritData spirit; // ссылка на духа
    
  public bool IsActive => Time.time < endTime;
  public float TimeLeft => Mathf.Max(0, endTime - Time.time);
}