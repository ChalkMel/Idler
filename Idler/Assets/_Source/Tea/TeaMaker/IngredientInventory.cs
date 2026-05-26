using System.Collections.Generic;
using UnityEngine;

public class IngredientInventory : MonoBehaviour
{
  [SerializeField] private Credits _credits;

  public bool HasIngredient(IngredientData ingredient)
  {
    if (_credits == null) return false;
    return ingredient.type switch
    {
      IngredientType.Berry => _credits.berries > 0,
      IngredientType.Flower => _credits.flowers > 0,
      IngredientType.Leaf => _credits.leaves > 0,
      _ => false
    };
  }

  public void UseIngredient(IngredientData ingredient)
  {
    if (_credits == null) return;
    switch (ingredient.type)
    {
      case IngredientType.Berry:
        _credits.berries--;
        break;
      case IngredientType.Flower:
        _credits.flowers--;
        break;
      case IngredientType.Leaf:
        _credits.leaves--;
        break;
    }
  }

  public void ReturnIngredients(List<IngredientData> ingredients)
  {
    if (_credits == null) return;
    foreach (var ingredient in ingredients)
    {
      switch (ingredient.type)
      {
        case IngredientType.Berry:
          _credits.berries++;
          break;
        case IngredientType.Flower:
          _credits.flowers++;
          break;
        case IngredientType.Leaf:
          _credits.leaves++;
          break;
      }
    }
  }
}