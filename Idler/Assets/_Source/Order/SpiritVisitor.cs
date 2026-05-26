using UnityEngine;
using System.Collections.Generic;

public class SpiritVisitor : MonoBehaviour
{
  [SerializeField] private VisitorSpawner _spawner;
  [SerializeField] private OrderMatcher _matcher;
    
  public bool IsWaitingForTea => _spawner != null && _spawner.IsWaitingForTea;
  public List<TeaData> RequestedTeas => _matcher?.CurrentOrder?.requestedTeas;
    
  public string SetOrderText()
  {
    return _matcher != null ? _matcher.GetOrderText() : "No active order";
  }
}