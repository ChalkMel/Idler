using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class OrderGenerator : MonoBehaviour
{
  [SerializeField] private TeaBrewingController teaMaker;
  [SerializeField] private SpiritCollection spiritCollection;
  [SerializeField] private int minTeasInOrder = 1;
  [SerializeField] private int maxTeasInOrder = 3;
    
  private System.Random _random = new System.Random();
    
  public OrderData GenerateOrder()
  {
    if (spiritCollection == null || spiritCollection.AllSpirits == null || spiritCollection.AllSpirits.Count == 0)
    {
      return null;
    }

    SpiritData visitor = spiritCollection.AllSpirits[_random.Next(spiritCollection.AllSpirits.Count)];
    
    int teasCount = _random.Next(minTeasInOrder, maxTeasInOrder + 1);
    
    List<TeaData> requestedTeas = new List<TeaData>();
    for (int i = 0; i < teasCount; i++)
    {
      if (teaMaker != null && teaMaker.allTeas != null && teaMaker.allTeas.Count > 0)
      {
        TeaData randomTea = teaMaker.allTeas[_random.Next(teaMaker.allTeas.Count)];
        requestedTeas.Add(randomTea);
      }
    }
        
    return new OrderData(visitor, requestedTeas);
  }
}