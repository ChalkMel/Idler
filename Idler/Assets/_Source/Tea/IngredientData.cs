using UnityEngine;

[CreateAssetMenu(fileName = "NewIngredient", menuName = "Tea/Ingredient")]
public class IngredientData : ScriptableObject
{
  public string IngredientName {get; private set; }
  public Sprite Icon {get; private set; }
  public IngredientType Type {get; private set; }
}

public enum IngredientType
{
  Leaf,
  Berry,
  Flower
}