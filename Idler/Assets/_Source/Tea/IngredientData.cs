// IngredientData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewIngredient", menuName = "Tea/Ingredient")]
public class IngredientData : ScriptableObject
{
  public string ingredientName;
  public Sprite icon;
  public IngredientType type;
}

public enum IngredientType
{
  Leaf,
  Berry,
  Flower
}