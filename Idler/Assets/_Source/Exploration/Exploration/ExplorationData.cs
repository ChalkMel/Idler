using System;

[Serializable]
public class ExplorationData
{
  public ZoneData Zone {get; private set; }
  public float TimeRemaining;
  public bool IsExploring;
    
  public ExplorationData(ZoneData targetZone)
  {
    Zone = targetZone;
    TimeRemaining = targetZone.ExplorationTime;
    IsExploring = true;
  }
    
  public void Update(float deltaTime)
  {
    if (IsExploring)
    {
      TimeRemaining -= deltaTime;
      if (TimeRemaining <= 0)
      {
        TimeRemaining = 0;
        IsExploring = false;
      }
    }
  }
    
  public bool IsComplete => !IsExploring && TimeRemaining <= 0;
  public float Progress => 1f - (TimeRemaining / Zone.ExplorationTime);
}