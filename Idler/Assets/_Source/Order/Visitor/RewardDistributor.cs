using UnityEngine;
using UnityEngine.Serialization;

public class RewardDistributor : MonoBehaviour
{
  [SerializeField] private Credits credits;
  [SerializeField] private int baseReward = 50;
  [SerializeField] private float rewardMultiplier = 1f;
    
  public void GiveReward(OrderData order)
  {
    if (credits == null || order == null) return;
        
    int reward = Mathf.RoundToInt(baseReward * rewardMultiplier * order.requestedTeas.Count);
    credits.droplets += reward;
    credits.UpdateUI();
  }
}