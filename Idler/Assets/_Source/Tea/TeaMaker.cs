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
    
    [Header("Tea Recipes")]
    [SerializeField] public List<TeaData> allTeas = new List<TeaData>();
    
    [Header("UI Elements")]
    [SerializeField] private Button brewButton;
    [SerializeField] private Button clearButton;
    [SerializeField] private Button cancelButton; // Кнопка отмены варки (опционально)
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
    
    [Header("Brewing Settings")]
    [SerializeField] private float brewingTime = 3f;
    [SerializeField] private Slider brewingSlider;
    [SerializeField] private TextMeshProUGUI brewingTimerText;
    [SerializeField] private GameObject brewingPanel; // Панель для элементов варки
    
    private List<IngredientData> currentIngredients = new List<IngredientData>();
    private bool isBrewing = false;
    private Coroutine brewingCoroutine;
    
    private void Start()
    {
        Debug.Log("TeaMaker Start - Initializing...");
        
        // Проверяем все необходимые ссылки
        if (!CheckReferences())
        {
            Debug.LogError("TeaMaker: Some references are missing!");
            return;
        }
        
        // Инициализируем кнопки ингредиентов
        InitializeIngredientButtons();
        
        // Настраиваем основные кнопки
        brewButton.onClick.AddListener(StartBrewing);
        clearButton.onClick.AddListener(ClearCauldron);
        
        // Настраиваем кнопку отмены если есть
        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(CancelBrewing);
            cancelButton.gameObject.SetActive(false);
        }
        
        // Обновляем счетчики
        UpdateCounters();
        
        // Скрываем UI элементы
        HideResult();
        ClearLikedSpiritsPanel();
        HideBrewingUI();
        
        Debug.Log("TeaMaker initialized successfully!");
        
        // Отладочная информация
        if (playerSpirits != null)
        {
            Debug.Log($"Player has {playerSpirits.unlockedSpirits?.Count ?? 0} unlocked spirits");
        }
    }
    
    // Проверка всех ссылок
    private bool CheckReferences()
    {
        bool allValid = true;
        
        if (credits == null)
        {
            Debug.LogError("TeaMaker: Credits reference is missing!");
            allValid = false;
        }
        
        if (playerSpirits == null)
        {
            Debug.LogWarning("TeaMaker: PlayerSpirits reference is missing - spirit checks won't work");
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
    
    // Инициализация кнопок ингредиентов
    private void InitializeIngredientButtons()
    {
        if (ingredientButtonsPanel == null) return;
        
        // Получаем все компоненты IngredientButton
        var ingredientButtons = ingredientButtonsPanel.GetComponentsInChildren<IngredientButton>();
        Debug.Log($"Found {ingredientButtons.Length} ingredient buttons");
        
        foreach (var ingredientButton in ingredientButtons)
        {
            // Проверяем что компонент существует
            if (ingredientButton == null)
            {
                Debug.LogWarning("Found null IngredientButton component");
                continue;
            }
            
            // Получаем Button компонент
            Button button = ingredientButton.GetComponent<Button>();
            if (button == null)
            {
                Debug.LogWarning("IngredientButton doesn't have Button component");
                continue;
            }
            
            // Получаем данные ингредиента
            IngredientData ingredient = ingredientButton.ingredientData;
            if (ingredient == null)
            {
                Debug.LogWarning("IngredientButton has no IngredientData assigned");
                continue;
            }
            
            // Назначаем обработчик с локальной переменной
            IngredientData localIngredient = ingredient;
            button.onClick.AddListener(() => AddIngredient(localIngredient));
            
            Debug.Log($"Set up button for {ingredient.ingredientName}");
        }
    }
    
    // Добавление ингредиента в котел
    public void AddIngredient(IngredientData ingredient)
    {
        if (ingredient == null)
        {
            Debug.LogError("Trying to add null ingredient!");
            return;
        }
        
        if (isBrewing)
        {
            ShowMessage("Дождитесь окончания варки!");
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
        
        // Добавляем ингредиент
        currentIngredients.Add(ingredient);
        
        // Создаем визуальное представление
        if (ingredientUIPrefab != null && ingredientsPanel != null)
        {
            var ui = Instantiate(ingredientUIPrefab, ingredientsPanel);
            var image = ui.GetComponentInChildren<Image>();
            if (image != null && ingredient.icon != null)
            {
                image.sprite = ingredient.icon;
                image.preserveAspect = true;
            }
        }
        
        // Используем ингредиент
        UseIngredient(ingredient);
        HideResult();
        ClearLikedSpiritsPanel();
    }
    
    // Проверка наличия ингредиента
    private bool HasIngredient(IngredientData ingredient)
    {
        if (credits == null) return false;
        
        switch (ingredient.type)
        {
            case IngredientType.Berry:
                return credits.berries > 0;
            case IngredientType.Flower:
                return credits.flowers > 0;
            case IngredientType.Leaf:
                return credits.leaves > 0;
            default:
                return false;
        }
    }
    
    // Использование ингредиента
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
    
    // Обновление счетчиков
    public void UpdateCounters()
    {
        if (credits == null) return;
        
        if (berryCount != null) berryCount.text = credits.berries.ToString();
        if (flowerCount != null) flowerCount.text = credits.flowers.ToString();
        if (leafCount != null) leafCount.text = credits.leaves.ToString();
    }
    
    // Начало варки чая
    public void StartBrewing()
    {
        if (isBrewing)
        {
            ShowMessage("Уже варится!");
            return;
        }
        
        if (currentIngredients.Count == 0)
        {
            ShowMessage("Котел пуст!");
            return;
        }
        
        // Отключаем кнопки на время варки
        brewButton.interactable = false;
        clearButton.interactable = false;
        SetIngredientButtonsInteractable(false);
        
        // Активируем кнопку отмены если есть
        if (cancelButton != null)
            cancelButton.gameObject.SetActive(true);
        
        isBrewing = true;
        ShowMessage("Начинаем варить чай...");
        
        // Запускаем корутину варки
        brewingCoroutine = StartCoroutine(BrewingCoroutine());
    }
    
    // Корутина варки чая
    private IEnumerator BrewingCoroutine()
    {
        float timer = 0f;
        
        // Показываем UI варки
        ShowBrewingUI();
        
        while (timer < brewingTime)
        {
            timer += Time.deltaTime;
            
            // Обновляем прогресс
            float progress = timer / brewingTime;
            if (brewingSlider != null)
                brewingSlider.value = progress;
            
            if (brewingTimerText != null)
                brewingTimerText.text = $"Варка... {Mathf.CeilToInt(brewingTime - timer)}с";
            
            yield return null;
        }
        
        // Завершаем варку
        CompleteBrewing();
    }
    
    // Завершение варки
    private void CompleteBrewing()
    {
        TeaData result = FindMatchingTea();
        
        if (result != null)
        {
            Debug.Log($"Found matching tea: {result.teaName}");
            
            // Проверяем, есть ли духи, которым нравится этот чай
            List<SpiritData> likedSpirits = result.GetLikedSpiritsForPlayer(playerSpirits);
            
            Debug.Log($"Liked spirits for this tea: {likedSpirits.Count}");
            
            if (likedSpirits.Count > 0)
            {
                // Успех: есть духи, которым нравится чай
                ShowSuccess(result);
                ShowLikedSpirits(likedSpirits);
                ShowMessage($"Успех! Приготовлен {result.teaName}");
                
                // Применяем бусты от духов
                ApplySpiritBuffs(likedSpirits);
            }
            else
            {
                // Нет духов, которым нравится этот чай
                ShowMessage("Пока этот чай никому из ваших духов не нравится!");
                ReturnIngredients();
            }
        }
        else
        {
            // Рецепт не найден
            ShowMessage("Не получилось!");
            ReturnIngredients();
        }
        
        // Очищаем котел
        CleanupAfterBrewing();
        // В CompleteBrewing методе добавьте:
        if (result != null && playerSpirits != null)
        {
            Debug.Log($"Tea '{result.teaName}' liked by {result.likedBySpirits?.Count ?? 0} spirits total");
            Debug.Log($"Player has {playerSpirits.unlockedSpirits?.Count ?? 0} unlocked spirits");
    
            if (result.likedBySpirits != null && playerSpirits.unlockedSpirits != null)
            {
                foreach (var spirit in result.likedBySpirits)
                {
                    bool isUnlocked = playerSpirits.unlockedSpirits.Contains(spirit);
                    Debug.Log($"  - {spirit?.spiritName ?? "null"}: {(isUnlocked ? "UNLOCKED" : "LOCKED")}");
                }
            }
        }
    }
    
    // Применение бустов от духов
    private void ApplySpiritBuffs(List<SpiritData> spirits)
    {
        if (spirits == null || spirits.Count == 0) return;
        
        float totalMultiplier = 1.0f;
        List<string> buffMessages = new List<string>();
        
        foreach (var spirit in spirits)
        {
            if (spirit == null) continue;
            
            totalMultiplier *= spirit.buffMultiplier;
            buffMessages.Add($"{spirit.spiritName}: {spirit.buffName} (x{spirit.buffMultiplier:F1})");
            
            // Применяем индивидуальные эффекты духов
            ApplyIndividualSpiritEffect(spirit);
        }
        
        // Показываем информацию о бустах
        if (buffMessages.Count > 0)
        {
            string buffInfo = "Активированы бусты:\n" + string.Join("\n", buffMessages);
            Debug.Log(buffInfo);
        }
        
        Debug.Log($"Общий множитель бустов: {totalMultiplier:F2}x");
    }
    
    // Индивидуальные эффекты духов
    private void ApplyIndividualSpiritEffect(SpiritData spirit)
    {
        // Здесь можно добавить специфические эффекты для каждого духа
        // Например, увеличение определенных ресурсов или особые бонусы
        
        switch (spirit.spiritName)
        {
            // Примеры:
            // case "Лесной дух":
            //     credits.berries += 2; // Добавляет дополнительные ягоды
            //     break;
            // case "Цветочный дух":
            //     credits.flowers += 2; // Добавляет дополнительные цветы
            //     break;
        }
    }
    
    // Показ духов, которым нравится чай
    private void ShowLikedSpirits(List<SpiritData> spirits)
    {
        ClearLikedSpiritsPanel();
        
        if (likedSpiritsPanel == null || spiritIconPrefab == null || spirits == null || spirits.Count == 0)
            return;
            
        foreach (var spirit in spirits)
        {
            if (spirit == null) continue;
            
            GameObject spiritIcon = Instantiate(spiritIconPrefab, likedSpiritsPanel);
            Image image = spiritIcon.GetComponent<Image>();
            if (image != null && spirit.icon != null)
            {
                image.sprite = spirit.icon;
                image.preserveAspect = true;
            }
        }
    }
    
    // Очистка после варки
    private void CleanupAfterBrewing()
    {
        currentIngredients.Clear();
        ClearVisuals();
        ClearLikedSpiritsPanel();
        
        // Завершаем состояние варки
        isBrewing = false;
        HideBrewingUI();
        
        // Восстанавливаем кнопки
        brewButton.interactable = true;
        clearButton.interactable = true;
        SetIngredientButtonsInteractable(true);
        
        // Скрываем кнопку отмены
        if (cancelButton != null)
            cancelButton.gameObject.SetActive(false);
    }
    
    // Отмена варки
    public void CancelBrewing()
    {
        if (!isBrewing) return;
        
        if (brewingCoroutine != null)
        {
            StopCoroutine(brewingCoroutine);
            brewingCoroutine = null;
        }
        
        isBrewing = false;
        HideBrewingUI();
        
        // Возвращаем ингредиенты при отмене
        ReturnIngredients();
        currentIngredients.Clear();
        ClearVisuals();
        ClearLikedSpiritsPanel();
        
        // Восстанавливаем кнопки
        brewButton.interactable = true;
        clearButton.interactable = true;
        SetIngredientButtonsInteractable(true);
        
        // Скрываем кнопку отмены
        if (cancelButton != null)
            cancelButton.gameObject.SetActive(false);
        
        ShowMessage("Варка отменена");
    }
    
    // Очистка котла (ручная)
    public void ClearCauldron()
    {
        if (isBrewing)
        {
            ShowMessage("Дождитесь окончания варки!");
            return;
        }
        
        ReturnIngredients();
        currentIngredients.Clear();
        ClearVisuals();
        ClearLikedSpiritsPanel();
        ShowMessage("Котел очищен");
    }
    
    // Поиск подходящего рецепта
    private TeaData FindMatchingTea()
    {
        if (allTeas == null) 
        {
            Debug.Log("AllTeas list is null!");
            return null;
        }
        
        Debug.Log($"Checking {allTeas.Count} teas against {currentIngredients.Count} ingredients");
        
        foreach (var tea in allTeas)
        {
            if (tea == null) 
            {
                Debug.LogWarning("Found null tea in allTeas list");
                continue;
            }
            
            if (tea.Matches(currentIngredients))
            {
                Debug.Log($"Found match: {tea.teaName}");
                return tea;
            }
        }
        
        Debug.Log("No matching tea found");
        return null;
    }
    
    // Показ успешного результата
    private void ShowSuccess(TeaData tea)
    {
        if (resultIcon != null)
        {
            if (tea.icon != null)
            {
                resultIcon.sprite = tea.icon;
                resultIcon.gameObject.SetActive(true);
                resultIcon.preserveAspect = true;
            }
        }
        
        if (resultName != null)
        {
            resultName.text = tea.teaName;
            resultName.gameObject.SetActive(true);
        }
    }
    
    // Скрытие результата
    private void HideResult()
    {
        if (resultIcon != null)
            resultIcon.gameObject.SetActive(false);
        
        if (resultName != null)
            resultName.gameObject.SetActive(false);
    }
    
    // Очистка визуальных элементов
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
    
    // Возврат ингредиентов
    private void ReturnIngredients()
    {
        if (credits == null) return;
        
        foreach (var ingredient in currentIngredients)
        {
            switch (ingredient.type)
            {
                case IngredientType.Berry:
                    credits.berries++;
                    break;
                case IngredientType.Flower:
                    credits.flowers++;
                    break;
                case IngredientType.Leaf:
                    credits.leaves++;
                    break;
            }
        }
        UpdateCounters();
    }
    
    // Управление доступностью кнопок ингредиентов
    private void SetIngredientButtonsInteractable(bool interactable)
    {
        var ingredientButtons = ingredientButtonsPanel.GetComponentsInChildren<Button>();
        foreach (var button in ingredientButtons)
        {
            button.interactable = interactable;
        }
    }
    
    // Показ UI варки
    private void ShowBrewingUI()
    {
        if (brewingPanel != null)
            brewingPanel.SetActive(true);
        
        if (brewingSlider != null)
        {
            brewingSlider.gameObject.SetActive(true);
            brewingSlider.value = 0f;
        }
        
        if (brewingTimerText != null)
        {
            brewingTimerText.gameObject.SetActive(true);
            brewingTimerText.text = $"Варка... {brewingTime}с";
        }
    }
    
    // Скрытие UI варки
    private void HideBrewingUI()
    {
        if (brewingPanel != null)
            brewingPanel.SetActive(false);
        
        if (brewingSlider != null)
            brewingSlider.gameObject.SetActive(false);
        
        if (brewingTimerText != null)
            brewingTimerText.gameObject.SetActive(false);
    }
    
    // Очистка панели духов
    private void ClearLikedSpiritsPanel()
    {
        if (likedSpiritsPanel == null) return;
        
        foreach (Transform child in likedSpiritsPanel)
        {
            Destroy(child.gameObject);
        }
    }
    
    // Показ сообщений
    private void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
            
            // Автоматически очищаем сообщение через 3 секунды
            CancelInvoke(nameof(ClearMessage));
            Invoke(nameof(ClearMessage), 3f);
        }
        else
        {
            Debug.Log(message);
        }
    }
    
    // Очистка сообщения
    private void ClearMessage()
    {
        if (messageText != null)
            messageText.text = "";
    }
    
    // Проверка на варку при выходе
    private void OnDestroy()
    {
        if (isBrewing && brewingCoroutine != null)
        {
            StopCoroutine(brewingCoroutine);
        }
    }
    
}