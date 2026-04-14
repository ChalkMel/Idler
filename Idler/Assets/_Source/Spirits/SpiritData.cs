using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSpirit", menuName = "Spirit")]
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
    
  [Header("Relationships")]
  public List<SpiritRelationship> relationships = new List<SpiritRelationship>();
    
  public RelationshipType GetRelationshipWith(SpiritData otherSpirit)
  {
    foreach (var relationship in relationships)
    {
      if (relationship.otherSpirit == otherSpirit)
        return relationship.relationshipType;
    }
    return RelationshipType.Neutral;
  }
}