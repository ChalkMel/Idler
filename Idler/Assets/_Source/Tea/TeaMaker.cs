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
    [SerializeField] private Image resultSpiritIcon; // Иконка духа
    [SerializeField] private TextMeshProUGUI resultBuffInfo; // Информация о бусте
    
    [Header("Brewing UI")]
    [SerializeField] private GameObject brewingPanel;
    [SerializeField] private Slider brewingSlider;
    [SerializeField] private TextMeshProUGUI brewingTimeText;
    [SerializeField] private TextMeshProUGUI brewingTeaNameText;
    
    private List<IngredientData> _currentIngredients = new List<IngredientData>();
    private bool _isBrewing;
    private Coroutine _brewingCoroutine;
    
    private void Start()
    {
        InitializeIngredientButtons();

        brewButton.onClick.AddListener(StartBrewing);
        clearButton.onClick.AddListener(ClearCauldron);

        UpdateCounters();
        
        // Скрываем панели
        if (brewingPanel != null) brewingPanel.SetActive(false);
        if (teaResultPanel != null) teaResultPanel.SetActive(false);
        
        // Обновляем доступность кнопки варки
        UpdateBrewButtonState();
    }
    
    private void Update()
    {
        // Постоянно обновляем состояние кнопки варки
        UpdateBrewButtonState();
    }
    
    private void UpdateBrewButtonState()
    {
        if (brewButton == null) return;
        
        // Если есть активный буст - кнопка варки неактивна
        if (buffManager != null && buffManager.HasActiveBuff())
        {
            brewButton.interactable = false;
            
            // Меняем текст кнопки
            TextMeshProUGUI buttonText = brewButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = "БУСТ АКТИВЕН";
            }
        }
        else
        {
            brewButton.interactable = !_isBrewing;
            
            // Возвращаем обычный текст
            TextMeshProUGUI buttonText = brewButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = "ПРИГОТОВИТЬ";
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
            ShowMessage("Дождитесь окончания варки!");
            return;
        }
        
        if (buffManager != null && buffManager.HasActiveBuff())
        {
            ShowMessage("Дождитесь окончания буста!");
            return;
        }
        
        if (!HasIngredient(ingredient))
        {
            ShowMessage($"Нет {ingredient.ingredientName}!");
            return;
        }
        
        if (_currentIngredients.Count >= 6)
        {
            ShowMessage("Максимум 6 ингредиентов!");
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
            ShowMessage("Уже варится!");
            return;
        }
        
        if (buffManager != null && buffManager.HasActiveBuff())
        {
            ShowMessage("Нельзя варить при активном бусте!");
            return;
        }
        
        if (_currentIngredients.Count == 0)
        {
            ShowMessage("Котел пуст!");
            return;
        }
        
        TeaData teaToBrew = FindMatchingTea();
        
        if (teaToBrew == null)
        {
            ShowMessage("Не получилось!");
            ReturnIngredients();
            ClearVisuals();
            _currentIngredients.Clear();
            return;
        }
        
        _isBrewing = true;
        brewButton.interactable = false;
        clearButton.interactable = false;
        SetIngredientButtonsInteractable(false);
        
        _brewingCoroutine = StartCoroutine(BrewingCoroutine(teaToBrew));
    }
    
    private IEnumerator BrewingCoroutine(TeaData tea)
    {
        // Показываем панель варки
        if (brewingPanel != null)
        {
            brewingPanel.SetActive(true);
            
            if (brewingTeaNameText != null)
                brewingTeaNameText.text = $"Варится: {tea.teaName}";
            
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
                brewingTimeText.text = $"Осталось: {Mathf.CeilToInt(brewingTime - timer)}с";
            
            yield return null;
        }
        
        CompleteBrewing(tea);
    }
    
    private void CompleteBrewing(TeaData tea)
    {
        if (brewingPanel != null)
            brewingPanel.SetActive(false);
        
        List<SpiritData> likedSpirits = tea.GetLikedSpiritsForPlayer(playerSpirits);
        
        if (likedSpirits.Count > 0)
        {
            // Выбираем случайного духа из тех, кому нравится чай
            SpiritData chosenSpirit = likedSpirits[Random.Range(0, likedSpirits.Count)];
            
            // Показываем результат с выбранным духом
            ShowTeaResult(tea, chosenSpirit);
            
            // Применяем буст выбранного духа
            ApplySpiritBuff(chosenSpirit);
            
            // Даем награду
            GiveReward(chosenSpirit);
            
            ShowMessage($"Успех! Приготовлен {tea.teaName}");
        }
        else
        {
            ShowMessage("Пока этот чай никому из ваших духов не нравится!");
            ReturnIngredients();
        }
        
        _isBrewing = false;
        clearButton.interactable = true;
        SetIngredientButtonsInteractable(true);
        
        ClearCauldron();
    }
    
    private void ApplySpiritBuff(SpiritData spirit)
    {
        if (buffManager == null || spirit == null) return;
        
        // Применяем буст духа
        buffManager.AddBuff(spirit);
        
        // Также можно добавить мгновенный эффект
        ApplyInstantSpiritEffect(spirit);
    }
    
    private void ApplyInstantSpiritEffect(SpiritData spirit)
    {
        if (credits == null || spirit == null) return;
        
        // Мгновенные бонусы в зависимости от типа духа
        switch (spirit.spiritName)
        {
            case "Лесной дух":
                credits.berries += 3;
                ShowMessage("+3 ягоды от Лесного духа!");
                break;
            case "Цветочный дух":
                credits.flowers += 3;
                ShowMessage("+3 цветка от Цветочного духа!");
                break;
            case "Лиственный дух":
                credits.leaves += 3;
                ShowMessage("+3 листочка от Лиственного духа!");
                break;
            case "Ягодный дух":
                credits.berries += 5;
                ShowMessage("+5 ягод от Ягодного духа!");
                break;
        }
        
        UpdateCounters();
    }
    
    private void ShowTeaResult(TeaData tea, SpiritData spirit)
    {
        if (teaResultPanel == null) return;

        // Настраиваем информацию о чае
        if (resultTeaIcon != null && tea.icon != null)
        {
            resultTeaIcon.sprite = tea.icon;
        }
        
        if (resultTeaName != null)
        {
            resultTeaName.text = tea.teaName;
        }
        
        if (resultTeaDescription != null)
        {
            resultTeaDescription.text = tea.description;
        }
        
        // Настраиваем информацию о духе
        if (resultSpiritIcon != null && spirit.icon != null)
        {
            resultSpiritIcon.sprite = spirit.icon;
            resultSpiritIcon.gameObject.SetActive(true);
        }
        
        if (resultBuffInfo != null)
        {
            resultBuffInfo.text = $"{spirit.spiritName}\n{spirit.buffName}\nx{spirit.buffMultiplier:F1} на {spirit.buffDuration}с";
            resultBuffInfo.gameObject.SetActive(true);
        }
        
        teaResultPanel.SetActive(true);

        StartCoroutine(HideResultPanelAfterDelay(4f));
    }
    
    private IEnumerator HideResultPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (teaResultPanel != null)
            teaResultPanel.SetActive(false);
    }
    
    private void GiveReward(SpiritData spirit)
    {
        if (credits == null || spirit == null) return;
        
        // Базовая награда
        int baseReward = 10;
        
        // Умножаем на силу буста духа
        int finalReward = Mathf.RoundToInt(baseReward * spirit.buffMultiplier);
        
        // Добавляем капли
        credits.droplets += finalReward;
        
        // Обновляем UI если есть метод UpdateUI
        if (credits is MonoBehaviour creditMono)
        {
            creditMono.Invoke("UpdateUI", 0.1f);
        }
        
        ShowMessage($"Получено {finalReward} капель (x{spirit.buffMultiplier:F1})!");
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
    
    private void OnDestroy()
    {
        if (_brewingCoroutine != null)
            StopCoroutine(_brewingCoroutine);
    }
}