// SpiritData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpirit", menuName = "Tea/Spirit")]
public class SpiritData : ScriptableObject
{
  public string spiritName;
  public Sprite spiritIcon;
  public string description;
    
  [Header("Preferences")]
  public bool unlocked = false;
}