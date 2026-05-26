using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class TeaRecipeMatcher
{
    public TeaData FindMatchingTea(List<IngredientData> ingredients, List<TeaData> allTeas)
    {
        if (allTeas == null || ingredients == null || ingredients.Count == 0) 
            return null;

        Debug.Log($"Looking for tea match with {ingredients.Count} ingredients");
        
        foreach (var tea in allTeas)
        {
            if (tea == null) continue;
            if (Matches(tea, ingredients))
            {
                Debug.Log($"Found matching tea: {tea.teaName}");
                return tea;
            }
        }
        
        Debug.Log("No matching tea found!");
        return null;
    }

    private bool Matches(TeaData tea, List<IngredientData> inputIngredients)
    {
        if (inputIngredients.Count != tea.ingredients.Count) 
        {
            Debug.Log($"Count mismatch: {inputIngredients.Count} vs {tea.ingredients.Count}");
            return false;
        }
        
        List<IngredientData> remainingInput = new List<IngredientData>(inputIngredients);
        List<IngredientData> remainingRecipe = new List<IngredientData>(tea.ingredients);
        
        foreach (var recipeIng in tea.ingredients)
        {
            bool found = false;
            
            for (int i = 0; i < remainingInput.Count; i++)
            {
                if (remainingInput[i].type == recipeIng.type && 
                    remainingInput[i].ingredientName == recipeIng.ingredientName)
                {
                    // Нашли соответствие - удаляем оба ингредиента
                    remainingInput.RemoveAt(i);
                    found = true;
                    break;
                }
            }
            
            if (!found)
            {
                Debug.Log($"Ingredient not found: {recipeIng.ingredientName} (type: {recipeIng.type})");
                return false;
            }
        }
        
        Debug.Log("All ingredients matched!");
        return true;
    }
}