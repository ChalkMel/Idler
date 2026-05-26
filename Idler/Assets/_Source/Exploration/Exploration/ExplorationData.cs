using System;

[Serializable]
public class ExplorationData
{
  public ZoneData zone;
  public float timeRemaining;
  public bool isExploring;
    
  public ExplorationData(ZoneData targetZone)
  {
    zone = targetZone;
    timeRemaining = targetZone.explorationTime;
    isExploring = true;
  }
    
  public void Update(float deltaTime)
  {
    if (isExploring)
    {
      timeRemaining -= deltaTime;
      if (timeRemaining <= 0)
      {
        timeRemaining = 0;
        isExploring = false;
      }
    }
  }
    
  public bool IsComplete => !isExploring && timeRemaining <= 0;
  public float Progress => 1f - (timeRemaining / zone.explorationTime);
}