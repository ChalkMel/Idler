using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTeaRecipe", menuName = "Tea/Tea Recipe")]
public class TeaData : ScriptableObject
{
  [Header("Basic Info")]
  public string teaName;
  public Sprite icon;
  [TextArea(2, 4)] public string description;
    
  [Header("Recipe")]
  public List<IngredientData> ingredients = new List<IngredientData>();
    
  [Header("Liked By")]
  public List<Sprite> likedBySpirits = new List<Sprite>();
    
  public bool Matches(List<IngredientData> inputIngredients)
  {
    if (inputIngredients.Count != ingredients.Count)
      return false;
        
    // Считаем типы ингредиентов
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
}