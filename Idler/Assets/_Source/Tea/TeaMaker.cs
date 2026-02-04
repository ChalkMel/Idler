using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeaMaker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Credits credits;
    [SerializeField] private SpiritCollection playerSpirits;
    [SerializeField] private SpiritBuffManager buffManager;
    
    [Header("Tea Recipes")]
    [SerializeField] public List<TeaData> allTeas = new List<TeaData>();
    
    [Header("UI Elements")]
    [SerializeField] private Button brewButton;
    [SerializeField] private Button clearButton;
    [SerializeField] private Transform ingredientsPanel;
    [SerializeField] private Transform ingredientButtonsPanel;
    
    [Header("Counters")]
    [SerializeField] private TextMeshProUGUI berryCount, flowerCount, leafCount;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject ingredientUIPrefab;
    
    [Header("Result Display")]
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image resultIcon;
    [SerializeField] private TextMeshProUGUI resultName;
    [SerializeField] private Transform likedSpiritsPanel;
    [SerializeField] private GameObject spiritIconPrefab;
    
    private List<IngredientData> currentIngredients = new List<IngredientData>();
    
    private void Start()
    {
        if (!CheckReferences())
        {
            Debug.LogError("TeaMaker: Some references are missing!");
            return;
        }

        InitializeIngredientButtons();

        brewButton.onClick.AddListener(BrewTea);
        clearButton.onClick.AddListener(ClearCauldron);

        UpdateCounters();
        
        HideResult();
        ClearLikedSpiritsPanel();
        
        Debug.Log("TeaMaker initialized successfully!");
    }
    
    private bool CheckReferences()
    {
        bool allValid = true;
        
        if (credits == null)
        {
            Debug.LogError("TeaMaker: Credits reference is missing!");
            allValid = false;
        }
        
        if (ingredientButtonsPanel == null)
        {
            Debug.LogError("TeaMaker: IngredientButtonsPanel is missing!");
            allValid = false;
        }
        
        if (brewButton == null)
        {
            Debug.LogError("TeaMaker: BrewButton is missing!");
            allValid = false;
        }
        
        if (clearButton == null)
        {
            Debug.LogError("TeaMaker: ClearButton is missing!");
            allValid = false;
        }
        
        return allValid;
    }
    
    private void InitializeIngredientButtons()
    {
        if (ingredientButtonsPanel == null) return;
        
        var ingredientButtons = ingredientButtonsPanel.GetComponentsInChildren<IngredientButton>();
        
        foreach (var ingredientButton in ingredientButtons)
        {
            if (ingredientButton == null) continue;
            
            Button button = ingredientButton.GetComponent<Button>();
            if (button == null) continue;
            
            IngredientData ingredient = ingredientButton.ingredientData;
            if (ingredient == null) continue;
            
            button.onClick.AddListener(() => AddIngredient(ingredient));
        }
    }
    
    public void AddIngredient(IngredientData ingredient)
    {
        if (ingredient == null)
        {
            Debug.LogError("Trying to add null ingredient!");
            return;
        }
        
        if (!HasIngredient(ingredient))
        {
            ShowMessage($"Нет {ingredient.ingredientName}!");
            return;
        }
        
        if (currentIngredients.Count >= 6)
        {
            ShowMessage("Максимум 6 ингредиентов!");
            return;
        }
        
        currentIngredients.Add(ingredient);
        
        if (ingredientUIPrefab != null && ingredientsPanel != null)
        {
            var ui = Instantiate(ingredientUIPrefab, ingredientsPanel);
            var image = ui.GetComponentInChildren<Image>();
            if (image != null && ingredient.icon != null)
            {
                image.sprite = ingredient.icon;
            }
        }
        
        UseIngredient(ingredient);
        HideResult();
        ClearLikedSpiritsPanel();
    }
    
    private bool HasIngredient(IngredientData ingredient)
    {
        if (credits == null) return false;
        
        return ingredient.type switch
        {
            IngredientType.Berry => credits.berries > 0,
            IngredientType.Flower => credits.flowers > 0,
            IngredientType.Leaf => credits.leaves > 0,
            _ => false
        };
    }
    
    private void UseIngredient(IngredientData ingredient)
    {
        if (credits == null) return;
        
        switch (ingredient.type)
        {
            case IngredientType.Berry: 
                credits.berries--; 
                break;
            case IngredientType.Flower: 
                credits.flowers--; 
                break;
            case IngredientType.Leaf: 
                credits.leaves--; 
                break;
        }
        UpdateCounters();
    }
    
    public void UpdateCounters()
    {
        if (credits == null) return;
        
        if (berryCount != null) berryCount.text = credits.berries.ToString();
        if (flowerCount != null) flowerCount.text = credits.flowers.ToString();
        if (leafCount != null) leafCount.text = credits.leaves.ToString();
    }
    
    public void BrewTea()
    {
        if (currentIngredients.Count == 0)
        {
            ShowMessage("Котел пуст!");
            return;
        }
        
        TeaData result = FindMatchingTea();
        
        if (result != null)
        {
            List<SpiritData> likedSpirits = result.GetLikedSpiritsForPlayer(playerSpirits);
            
            if (likedSpirits.Count > 0)
            {
                ShowSuccess(result);
                ShowLikedSpirits(likedSpirits);
                ShowMessage($"Успех! Приготовлен {result.teaName}");
                
                ApplySpiritBuffs(likedSpirits);
                
                GiveReward(likedSpirits);
            }
            else
            {
                ShowMessage("Пока этот чай никому из ваших духов не нравится!");
                ReturnIngredients();
            }
        }
        else
        {
            ShowMessage("Не получилось!");
            ReturnIngredients();
        }
        
        ClearCauldron();
    }
    
    private TeaData FindMatchingTea()
    {
        if (allTeas == null) return null;
        
        foreach (var tea in allTeas)
        {
            if (tea == null) continue;
            if (tea.Matches(currentIngredients))
                return tea;
        }
        return null;
    }
    
    private void ShowLikedSpirits(List<SpiritData> spirits)
    {
        ClearLikedSpiritsPanel();
        
        if (likedSpiritsPanel == null || spiritIconPrefab == null) return;
        
        foreach (var spirit in spirits)
        {
            if (spirit == null) continue;
            
            GameObject spiritIcon = Instantiate(spiritIconPrefab, likedSpiritsPanel);
            Image image = spiritIcon.GetComponent<Image>();
            if (image != null && spirit.icon != null)
            {
                image.sprite = spirit.icon;
            }
        }
    }
    
    private void ClearLikedSpiritsPanel()
    {
        if (likedSpiritsPanel == null) return;
        
        foreach (Transform child in likedSpiritsPanel)
        {
            Destroy(child.gameObject);
        }
    }
    
    private void ApplySpiritBuffs(List<SpiritData> spirits)
    {
        if (buffManager == null || spirits == null) return;
        
        foreach (var spirit in spirits)
        {
            if (spirit == null) continue;
            
            buffManager.AddBuff(spirit);
            
            ApplyInstantSpiritEffect(spirit);
        }
    }
    
    private void ApplyInstantSpiritEffect(SpiritData spirit)
    {
        if (credits == null) return;
        
        switch (spirit.spiritName)
        {
            case "Лесной дух":
                credits.berries += 2;
                break;
            case "Цветочный дух":
                credits.flowers += 2;
                break;
            case "Лиственный дух":
                credits.leaves += 2;
                break;
            case "Ягодный дух":
                credits.berries += 3;
                break;
        }
        
        UpdateCounters();
    }
    
    private void GiveReward(List<SpiritData> spirits)
    {
        if (credits == null) return;
        
        int baseReward = 10;
        float multiplier = 1f;
        
        if (buffManager != null)
        {
            multiplier = buffManager.GetTotalMultiplier();
        }
        
        multiplier += spirits.Count * 0.1f;
        
        int finalReward = Mathf.RoundToInt(baseReward * multiplier);
        
        // 
        
        Debug.Log($"Награда за чай: {finalReward} (x{multiplier:F1})");
    }
    
    private void ShowSuccess(TeaData tea)
    {
        if (resultIcon != null && tea.icon != null)
        {
            resultIcon.sprite = tea.icon;
            resultIcon.gameObject.SetActive(true);
        }
        
        if (resultName != null)
        {
            resultName.text = tea.teaName;
            resultName.gameObject.SetActive(true);
        }
    }
    
    private void HideResult()
    {
        if (resultIcon != null)
            resultIcon.gameObject.SetActive(false);
        
        if (resultName != null)
            resultName.gameObject.SetActive(false);
    }
    
    public void ClearCauldron()
    {
        ReturnIngredients();
        currentIngredients.Clear();
        ClearVisuals();
        ClearLikedSpiritsPanel();
    }
    
    private void ClearVisuals()
    {
        if (ingredientsPanel != null)
        {
            foreach (Transform child in ingredientsPanel)
                Destroy(child.gameObject);
        }
        HideResult();
        UpdateCounters();
    }
    
    private void ReturnIngredients()
    {
        if (credits == null) return;
        
        foreach (var ingredient in currentIngredients)
        {
            switch (ingredient.type)
            {
                case IngredientType.Berry: credits.berries++; break;
                case IngredientType.Flower: credits.flowers++; break;
                case IngredientType.Leaf: credits.leaves++; break;
            }
        }
        UpdateCounters();
    }
    
    private void ShowMessage(string message)
    {
        if (messageText != null)
            messageText.text = message;
    }
}