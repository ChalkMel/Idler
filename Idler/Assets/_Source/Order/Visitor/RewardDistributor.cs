using UnityEngine;

public class RewardDistributor : MonoBehaviour
{
  [SerializeField] private Credits _credits;
  [SerializeField] private int _baseReward = 50;
  [SerializeField] private float _rewardMultiplier = 1f;
    
  public void GiveReward(OrderData order)
  {
    if (_credits == null || order == null) return;
        
    int reward = Mathf.RoundToInt(_baseReward * _rewardMultiplier * order.requestedTeas.Count);
    _credits.droplets += reward;
    _credits.UpdateUI();
  }
}