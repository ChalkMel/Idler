using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeaMaker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Credits credits;
    
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
    
    private List<IngredientData> currentIngredients = new List<IngredientData>();
    
    private void Start()
    {
        var ingredientButtons = ingredientButtonsPanel.GetComponentsInChildren<IngredientButton>();
        
        foreach (var ingredientButton in ingredientButtons)
        {
            
            Button button = ingredientButton.GetComponent<Button>();
            IngredientData ingredient = ingredientButton.ingredientData;
            button.onClick.AddListener(() => AddIngredient(ingredient));
            Debug.Log($"Set up button for {ingredient.ingredientName}");
        }
        
        brewButton.onClick.AddListener(BrewTea);
        clearButton.onClick.AddListener(ClearCauldron);
        
        UpdateCounters();
        HideResult();
    }
    
    public void AddIngredient(IngredientData ingredient)
    {
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
        
        var ui = Instantiate(ingredientUIPrefab, ingredientsPanel);
        var image = ui.GetComponentInChildren<Image>();
        if (image != null && ingredient.icon != null)
        {
            image.sprite = ingredient.icon;
        }
        
        UseIngredient(ingredient);
        HideResult();
    }
    
    private bool HasIngredient(IngredientData ingredient)
    {
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
    
    private void UpdateCounters()
    {
        if (berryCount != null) berryCount.text = credits.berries.ToString();
        if (flowerCount != null) flowerCount.text = credits.flowers.ToString();
        if (leafCount != null) leafCount.text = credits.leaves.ToString();
    }
    
    public void BrewTea()
    {
        ShowMessage("Brewing tea...");
    
        if (currentIngredients.Count == 0)
        {
            ShowMessage("Котел пуст!");
            return;
        }
    
        TeaData result = FindMatchingTea();
    
        if (result != null)
        {
            ShowSuccess(result);
            currentIngredients.Clear();
            ClearVisuals();
        }
        else
        {
            ShowMessage("Не получилось!");
            ReturnIngredients();
            currentIngredients.Clear();
            ClearVisuals();
        }
    }

    private void ClearVisuals()
    {
        if (ingredientsPanel != null)
        {
            foreach (Transform child in ingredientsPanel)
                Destroy(child.gameObject);
        }
        HideResult();
        UpdateCounters(); // Обновляем счетчики (особенно важно при возврате)
    }
    
    public void ClearCauldron()
    {
        ReturnIngredients();
        currentIngredients.Clear();
        ClearVisuals();
    }
    
    private TeaData FindMatchingTea()
    {
        foreach (var tea in allTeas)
        {
            if (tea == null) continue;
            if (tea.Matches(currentIngredients))
                return tea;
        }
        return null;
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
        else
            Debug.Log(message);
    }
}