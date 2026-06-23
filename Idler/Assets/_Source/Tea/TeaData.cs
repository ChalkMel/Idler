using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTea", menuName = "Tea/Tea Data")]
public class TeaData : ScriptableObject
{
        [Header("Basic Info")] 
        public string TeaName {get; private set; }
        public Sprite Icon {get; private set; }
        [TextArea(2, 4)] public string Description {get; private set; }

        [Header("Brewing")] 
        public float BrewingTime;
        public float baseBrewingTime;

        [Header("Recipe")] 
        public List<IngredientData> Ingredients {get; private set; } = new List<IngredientData>();

        [Header("Spirits who like this tea")] 
        public List<SpiritData> LikedBySpirits {get; private set; } = new List<SpiritData>();

        private void OnEnable()
        {
            if (baseBrewingTime == 0)
                baseBrewingTime = BrewingTime;
        }

        public void ResetBrewingTime()
        {
            BrewingTime = baseBrewingTime;
        }

        public bool Matches(List<IngredientData> inputIngredients)
        {
            if (inputIngredients.Count != Ingredients.Count)
                return false;

            int leafCount = 0, berryCount = 0, flowerCount = 0;
            int requiredLeaf = 0, requiredBerry = 0, requiredFlower = 0;

            foreach (var ing in inputIngredients)
            {
                switch (ing.Type)
                {
                    case IngredientType.Leaf: leafCount++; break;
                    case IngredientType.Berry: berryCount++; break;
                    case IngredientType.Flower: flowerCount++; break;
                }
            }

            foreach (var ing in Ingredients)
            {
                switch (ing.Type)
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

            if (playerSpirits == null || LikedBySpirits == null || playerSpirits.UnlockedSpirits == null)
                return result;

            foreach (var spirit in LikedBySpirits)
            {
                if (spirit != null && playerSpirits.AvailableSpirits.Contains(spirit))
                {
                    result.Add(spirit);
                }
            }

            return result;
        }
    }