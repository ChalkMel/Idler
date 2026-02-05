using System.Collections.Generic;
using System.Collections;
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
    [SerializeField] private GameObject teaResultPanel;
    [SerializeField] private Image resultTeaIcon;
    [SerializeField] private TextMeshProUGUI resultTeaName;
    [SerializeField] private TextMeshProUGUI resultTeaDescription;
    [SerializeField] private Image resultSpiritIcon;
    [SerializeField] private TextMeshProUGUI resultSpiritName;
    [SerializeField] private TextMeshProUGUI resultBuffInfo;
    
    [Header("Brewing UI")]
    [SerializeField] private GameObject brewingPanel;
    [SerializeField] private Slider brewingSlider;
    [SerializeField] private TextMeshProUGUI brewingTimeText;
    [SerializeField] private TextMeshProUGUI brewingTeaNameText;
    [SerializeField] private Image brewingTeaIcon;
    
    private List<IngredientData> _currentIngredients = new List<IngredientData>();
    private bool _isBrewing;
    private Coroutine _brewingCoroutine;
    private TeaData _currentBrewingTea;
    
    private void Start()
    {
        InitializeIngredientButtons();

        brewButton.onClick.AddListener(StartBrewing);
        clearButton.onClick.AddListener(ClearCauldron);

        UpdateCounters();
        
        if (brewingPanel != null) brewingPanel.SetActive(false);
        if (teaResultPanel != null) teaResultPanel.SetActive(false);
        
        UpdateBrewButtonState();
    }
    
    private void Update()
    {
        UpdateBrewButtonState();
    }
    
    private void UpdateBrewButtonState()
    {
        if (brewButton == null) return;
        
        if (buffManager != null && buffManager.HasActiveBuff())
        {
            brewButton.interactable = false;
            
            TextMeshProUGUI buttonText = brewButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = "Boost is Active";
            }
        }
        else
        {
            brewButton.interactable = !_isBrewing;
            
            TextMeshProUGUI buttonText = brewButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = "Make";
            }
        }
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

    private void AddIngredient(IngredientData ingredient)
    {
        if (_isBrewing)
        {
            ShowMessage("Wait until brew end!");
            return;
        }
        
        if (buffManager != null && buffManager.HasActiveBuff())
        {
            ShowMessage("Wait until boost end!");
            return;
        }
        
        if (!HasIngredient(ingredient))
        {
            ShowMessage($"You don't have {ingredient.ingredientName}!");
            return;
        }
        
        if (_currentIngredients.Count >= 6)
        {
            ShowMessage("Max is 6!");
            return;
        }
        
        _currentIngredients.Add(ingredient);
        
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
        if (berryCount != null) berryCount.text = credits.berries.ToString();
        if (flowerCount != null) flowerCount.text = credits.flowers.ToString();
        if (leafCount != null) leafCount.text = credits.leaves.ToString();
    }

    private void StartBrewing()
    {
        if (_isBrewing)
        {
            ShowMessage("Already making!");
            return;
        }
        
        if (buffManager != null && buffManager.HasActiveBuff())
        {
            ShowMessage("You can't brew while boost on!");
            return;
        }
        
        if (_currentIngredients.Count == 0)
        {
            ShowMessage("Add something in!");
            return;
        }
        
        _currentBrewingTea = FindMatchingTea();
        
        if (_currentBrewingTea == null)
        {
            ShowMessage("Something went wrong!");
            ReturnIngredients();
            ClearVisuals();
            _currentIngredients.Clear();
            return;
        }
        
        _isBrewing = true;
        brewButton.interactable = false;
        clearButton.interactable = false;
        SetIngredientButtonsInteractable(false);
        
        _brewingCoroutine = StartCoroutine(BrewingCoroutine(_currentBrewingTea));
    }
    
    private IEnumerator BrewingCoroutine(TeaData tea)
    {
        if (brewingPanel != null)
        {
            brewingPanel.SetActive(true);
            
            if (brewingTeaNameText != null)
                brewingTeaNameText.text = $"Brewing: {tea.teaName}";
            
            if (brewingTeaIcon != null && tea.icon != null)
            {
                brewingTeaIcon.sprite = tea.icon;
                brewingTeaIcon.gameObject.SetActive(true);
            }
            
            if (brewingSlider != null)
                brewingSlider.value = 0f;
        }
        
        float brewingTime = tea.brewingTime;
        float timer = 0f;
        
        while (timer < brewingTime)
        {
            timer += Time.deltaTime;
            float progress = timer / brewingTime;
            
            if (brewingSlider != null)
                brewingSlider.value = progress;
            
            if (brewingTimeText != null)
                brewingTimeText.text = $"Time left: {Mathf.CeilToInt(brewingTime - timer)}s";
            
            yield return null;
        }
        
        CompleteBrewing(tea);
    }
    
    private void CompleteBrewing(TeaData tea)
    {
        if (brewingPanel != null)
        {
            brewingPanel.SetActive(false);
            if (brewingTeaIcon != null)
                brewingTeaIcon.gameObject.SetActive(false);
        }
        
        List<SpiritData> likedSpirits = tea.GetLikedSpiritsForPlayer(playerSpirits);
        
        if (likedSpirits.Count > 0)
        {
            SpiritData chosenSpirit = likedSpirits[Random.Range(0, likedSpirits.Count)];
            
            ShowTeaAndSpiritResult(tea, chosenSpirit);
            
            ApplySpiritBuff(chosenSpirit);
            
            GiveReward(chosenSpirit);
            
            ShowMessage($"Hooray! Brewed {tea.teaName} and {chosenSpirit.spiritName} came");
        }
        else
        {
            ShowMessage("This tea not liked by any!");
            ReturnIngredients();
        }
        
        _isBrewing = false;
        clearButton.interactable = true;
        SetIngredientButtonsInteractable(true);
        
        ClearCauldron();
    }
    
    private void ShowTeaAndSpiritResult(TeaData tea, SpiritData spirit)
    {
        if (teaResultPanel == null) return;
        
        if (resultTeaIcon != null && tea.icon != null)
        {
            resultTeaIcon.sprite = tea.icon;
            resultTeaIcon.gameObject.SetActive(true);
        }
        
        if (resultTeaName != null)
        {
            resultTeaName.text = $"You made:\n{tea.teaName}";
            resultTeaName.gameObject.SetActive(true);
        }
        
        if (resultTeaDescription != null)
        {
            resultTeaDescription.text = tea.description;
            resultTeaDescription.gameObject.SetActive(true);
        }
        
        if (resultSpiritIcon != null && spirit.icon != null)
        {
            resultSpiritIcon.sprite = spirit.icon;
            resultSpiritIcon.gameObject.SetActive(true);
        }
        
        if (resultSpiritName != null)
        {
            resultSpiritName.text = $"Spirit that came:\n{spirit.spiritName}";
            resultSpiritName.gameObject.SetActive(true);
        }
        
        if (resultBuffInfo != null)
        {
            resultBuffInfo.text = $"Boost: {spirit.buffName}\nStrength: x{spirit.buffMultiplier:F1}\nDuration: {spirit.buffDuration}с";
            resultBuffInfo.gameObject.SetActive(true);
        }
        
        teaResultPanel.SetActive(true);

        StartCoroutine(HideResultPanelAfterDelay(5f));
    }
    
    private IEnumerator HideResultPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (teaResultPanel != null)
            teaResultPanel.SetActive(false);
        
        if (resultTeaIcon != null) resultTeaIcon.gameObject.SetActive(false);
        if (resultTeaName != null) resultTeaName.gameObject.SetActive(false);
        if (resultTeaDescription != null) resultTeaDescription.gameObject.SetActive(false);
        if (resultSpiritIcon != null) resultSpiritIcon.gameObject.SetActive(false);
        if (resultSpiritName != null) resultSpiritName.gameObject.SetActive(false);
        if (resultBuffInfo != null) resultBuffInfo.gameObject.SetActive(false);
    }
    
    private void ApplySpiritBuff(SpiritData spirit)
    {
        if (buffManager == null || spirit == null) return;
        
        buffManager.AddBuff(spirit);
        ApplyInstantSpiritEffect(spirit);
    }
    
    private void ApplyInstantSpiritEffect(SpiritData spirit)
    {
        if (credits == null || spirit == null) return;
        
        UpdateCounters();
    }
    
    private void GiveReward(SpiritData spirit)
    {
        if (credits == null || spirit == null) return;
        
        int baseReward = 10;
        int finalReward = Mathf.RoundToInt(baseReward * spirit.buffMultiplier);
        
        credits.droplets += finalReward;
        
        if (credits is MonoBehaviour creditMono)
        {
            creditMono.Invoke("UpdateUI", 0.1f);
        }
    }
    
    private TeaData FindMatchingTea()
    {
        if (allTeas == null) return null;
        
        foreach (var tea in allTeas)
        {
            if (tea == null) continue;
            if (tea.Matches(_currentIngredients))
                return tea;
        }
        return null;
    }
    
    private void SetIngredientButtonsInteractable(bool interactable)
    {
        if (ingredientButtonsPanel == null) return;
        
        var buttons = ingredientButtonsPanel.GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            button.interactable = interactable;
        }
    }
    
    private void ClearCauldron()
    {
        _currentIngredients.Clear();
        ClearVisuals();
    }
    
    private void ClearVisuals()
    { 
        if (ingredientsPanel != null)
        {
            foreach (Transform child in ingredientsPanel)
                Destroy(child.gameObject);
        }
        
        UpdateCounters();
    }
    
    private void ReturnIngredients()
    {
        if (credits == null) return;
        
        foreach (var ingredient in _currentIngredients)
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