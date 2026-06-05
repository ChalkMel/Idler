using System.Collections.Generic;
using UnityEngine;

public class OrderGenerator : MonoBehaviour
{
  [SerializeField] private TeaBrewingController _teaMaker;
  [SerializeField] private SpiritCollection _spiritCollection;
  [SerializeField] private int _minTeasInOrder = 1;
  [SerializeField] private int _maxTeasInOrder = 3;
    
  private System.Random _random = new System.Random();
    
  public OrderData GenerateOrder()
  {
    if (_spiritCollection == null || _spiritCollection.allSpirits == null || _spiritCollection.allSpirits.Count == 0)
    {
      return null;
    }

    SpiritData visitor = _spiritCollection.allSpirits[_random.Next(_spiritCollection.allSpirits.Count)];
    
    int teasCount = _random.Next(_minTeasInOrder, _maxTeasInOrder + 1);
    
    List<TeaData> requestedTeas = new List<TeaData>();
    for (int i = 0; i < teasCount; i++)
    {
      if (_teaMaker != null && _teaMaker.allTeas != null && _teaMaker.allTeas.Count > 0)
      {
        TeaData randomTea = _teaMaker.allTeas[_random.Next(_teaMaker.allTeas.Count)];
        requestedTeas.Add(randomTea);
      }
    }
        
    return new OrderData(visitor, requestedTeas);
  }
}