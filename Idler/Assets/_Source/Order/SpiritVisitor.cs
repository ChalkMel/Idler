using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class SpiritVisitor : MonoBehaviour
{
  [SerializeField] private VisitorSpawner spawner;
  [SerializeField] private OrderMatcher matcher;
    
  public bool IsWaitingForTea => spawner != null && spawner.IsWaitingForTea;
  public List<TeaData> RequestedTeas => matcher?.CurrentOrder?.requestedTeas;
    
  public string SetOrderText()
  {
    return matcher != null ? matcher.GetOrderText() : "No active order";
  }
}