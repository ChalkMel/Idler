using UnityEngine;

public class OrderMatcher : MonoBehaviour
{
  private OrderData _currentOrder;
    
  public event System.Action<TeaData, int, int> OnTeaAccepted;
  public event System.Action OnOrderCompleted;
    
  public bool HasActiveOrder => _currentOrder != null && !_currentOrder.IsComplete;
  public OrderData CurrentOrder => _currentOrder;
    
  public void SetOrder(OrderData order)
  {
    _currentOrder = order;
  }
    
  public bool TrySubmitTea(TeaData tea)
  {
    if (_currentOrder == null)
    {
      Debug.Log("No active order!");
      return false;
    }
        
    if (_currentOrder.TryCompleteTea(tea))
    {
      Debug.Log($"Tea {tea.teaName} accepted! Progress: {_currentOrder.completedCount}/{_currentOrder.requestedTeas.Count}");
      OnTeaAccepted?.Invoke(tea, _currentOrder.completedCount, _currentOrder.requestedTeas.Count);
            
      if (_currentOrder.IsComplete)
      {
        Debug.Log("Order completed!");
        OnOrderCompleted?.Invoke();
      }
            
      return true;
    }
        
    Debug.Log($"Tea {tea.teaName} not in order or already served!");
    return false;
  }
    
  public void ClearOrder()
  {
    _currentOrder = null;
  }
    
  public string GetOrderText()
  {
    return _currentOrder != null ? _currentOrder.GetOrderText() : "No active order";
  }
    
  public TeaData GetNextRequiredTea()
  {
    return _currentOrder != null ? _currentOrder.GetNextRequiredTea() : null;
  }
}