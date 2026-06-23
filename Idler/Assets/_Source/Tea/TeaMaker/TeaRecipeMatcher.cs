using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class TeaRecipeMatcher
{
    public TeaData FindMatchingTea(List<IngredientData> ingredients, List<TeaData> allTeas)
    {
        if (allTeas == null || ingredients == null || ingredients.Count == 0) 
            return null;
        
        foreach (var tea in allTeas)
        {
            if (tea == null) continue;
            if (Matches(tea, ingredients))
            {
                return tea;
            }
        }
        
        return null;
    }

    private bool Matches(TeaData tea, List<IngredientData> inputIngredients)
    {
        if (inputIngredients.Count != tea.Ingredients.Count) 
        {
            return false;
        }
        
        List<IngredientData> remainingInput = new List<IngredientData>(inputIngredients);
        List<IngredientData> remainingRecipe = new List<IngredientData>(tea.Ingredients);
        
        foreach (var recipeIng in tea.Ingredients)
        {
            bool found = false;
            
            for (int i = 0; i < remainingInput.Count; i++)
            {
                if (remainingInput[i].Type == recipeIng.Type && 
                    remainingInput[i].IngredientName == recipeIng.IngredientName)
                {
                    remainingInput.RemoveAt(i);
                    found = true;
                    break;
                }
            }
            
            if (!found)
            {
                return false;
            }
        }
        return true;
    }
}