using UnityEngine;
using System.Collections.Generic;
using Effects;

[CreateAssetMenu(fileName = "NewSpirit", menuName = "Spirit")]
public class SpiritData : ScriptableObject
{
  [Header("Basic Info")]
  public string SpiritName {get; private set;}
  public Sprite Icon {get; private set;}
  [TextArea(2, 4)] public string Description {get; private set;}
    
  [Header("Preferences")]
  public TeaData[] LikedTeas {get; private set;}
    
  [Header("Buff")]
  public string BuffName {get; private set;}
  [TextArea(1, 2)] public string BuffDescription {get; private set;}
  public SpiritEffect Effect  {get; private set;}
  public float BuffMultiplier  {get; private set;}
  public float BuffDuration  {get; private set; }
    
  [Header("Unlock")]
  public bool isUnlocked = false;
    
  [Header("Relationships")] 
  [SerializeField] private List<SpiritRelationship> _relationships = new List<SpiritRelationship>();
    
  public RelationshipType GetRelationshipWith(SpiritData otherSpirit)
  {
    foreach (var relationship in _relationships)
    {
      if (relationship.otherSpirit == otherSpirit)
        return relationship.relationshipType;
    }
    return RelationshipType.Neutral;
  }
}