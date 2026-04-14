using UnityEngine;

[System.Serializable]
public class SpiritRelationship
{
    public SpiritData otherSpirit;
    public RelationshipType relationshipType;
}

public enum RelationshipType
{
    Neutral,   
    Positive,  
    Negative    
}