using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTea", menuName = "Tea/Tea Data")]
public class TeaData : ScriptableObject
{
    [Header("Basic Info")]
    public string teaName;
    public Sprite icon;
    [TextArea(2, 4)] public string description;
    
    [Header("Brewing")]
    public float brewingTime = 5f; 
    
    [Header("Recipe")]
    public List<IngredientData> ingredients = new List<IngredientData>();
    
    [Header("Spirits who like this tea")]
    public List<SpiritData> likedBySpirits = new List<SpiritData>();
    
    public bool Matches(List<IngredientData> inputIngredients)
    {
        if (inputIngredients.Count != ingredients.Count)
            return false;
        
        int leafCount = 0, berryCount = 0, flowerCount = 0;
        int requiredLeaf = 0, requiredBerry = 0, requiredFlower = 0;
        
        foreach (var ing in inputIngredients)
        {
            switch (ing.type)
            {
                case IngredientType.Leaf: leafCount++; break;
                case IngredientType.Berry: berryCount++; break;
                case IngredientType.Flower: flowerCount++; break;
            }
        }
        
        foreach (var ing in ingredients)
        {
            switch (ing.type)
            {
                case IngredientType.Leaf: requiredLeaf++; break;
                case IngredientType.Berry: requiredBerry++; break;
                case IngredientType.Flower: requiredFlower++; break;
            }
        }
        
        return leafCount == requiredLeaf && 
               berryCount == requiredBerry && 
               flowerCount == requiredFlower;
    }
    
    public List<SpiritData> GetLikedSpiritsForPlayer(SpiritCollection playerSpirits)
    {
        List<SpiritData> result = new List<SpiritData>();
        
        if (playerSpirits == null || likedBySpirits == null || playerSpirits.unlockedSpirits == null)
            return result;
            
        foreach (var spirit in likedBySpirits)
        {
            if (spirit != null && playerSpirits.unlockedSpirits.Contains(spirit))
            {
                result.Add(spirit);
            }
        }
        return result;
    }
}