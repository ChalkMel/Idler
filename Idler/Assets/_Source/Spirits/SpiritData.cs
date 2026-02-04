using UnityEngine;

[CreateAssetMenu(fileName = "NewSpirit", menuName = "Tea/Spirit Data")]
public class SpiritData : ScriptableObject
{
  [Header("Basic Info")]
  public int spiritId;
  public string spiritName;
  public Sprite icon;
  [TextArea(2, 4)] public string description;
    
  [Header("Preferences")]
  public TeaData[] likedTeas;
    
  [Header("Buff")]
  public string buffName;
  [TextArea(1, 2)] public string buffDescription;
  public float buffMultiplier = 1.0f;
  public float buffDuration = 30f;
    
  [Header("Unlock")]
  public bool isUnlocked = false;
  public int unlockCost = 100;
}