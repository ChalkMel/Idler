using System.Collections.Generic;

[System.Serializable]
public class OrderData
{
  public SpiritData visitor;
  public List<TeaData> requestedTeas;
  public List<bool> completedTeas;
  public int completedCount;
    
  public int TotalCount => requestedTeas != null ? requestedTeas.Count : 0;
    
  public OrderData(SpiritData spirit, List<TeaData> teas)
  {
    visitor = spirit;
    requestedTeas = new List<TeaData>(teas);
    completedTeas = new List<bool>();
    for (int i = 0; i < teas.Count; i++)
      completedTeas.Add(false);
    completedCount = 0;
  }
    
  public bool TryCompleteTea(TeaData tea)
  {
    for (int i = 0; i < requestedTeas.Count; i++)
    {
      if (requestedTeas[i] == tea && !completedTeas[i])
      {
        completedTeas[i] = true;
        completedCount++;
        return true;
      }
    }
    return false;
  }
    
  public bool IsComplete => completedCount >= requestedTeas.Count;
    
  public TeaData GetNextRequiredTea()
  {
    for (int i = 0; i < requestedTeas.Count; i++)
    {
      if (!completedTeas[i])
        return requestedTeas[i];
    }
    return null;
  }
    
  public string GetOrderText()
  {
    if (IsComplete)
      return "Thank you! All teas were delicious!";
            
    string text = "Prepare for me:\n";
    for (int i = 0; i < requestedTeas.Count; i++)
    {
      if (!completedTeas[i])
        text += $"- {requestedTeas[i].teaName}\n";
    }
    return text;
  }
}