using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeaMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TeaMaker teaMaker;
    
    [Header("UI Elements")]
    [SerializeField] private Transform teaListParent;
    [SerializeField] private GameObject teaButtonPrefab;
    [SerializeField] private Image selectedTeaIcon;
    [SerializeField] private TextMeshProUGUI selectedTeaName;
    [SerializeField] private TextMeshProUGUI selectedTeaDescription;
    [SerializeField] private Transform recipeIngredientsPanel;
    [SerializeField] private Transform likedSpiritsPanel;
    [SerializeField] private GameObject iconPrefab;
    
    [Header("Settings")]
    [SerializeField] private bool startHidden = true;
    
    private List<GameObject> teaButtons = new List<GameObject>();
    
    private void Start()
    {
        if (startHidden)
        {
            gameObject.SetActive(false);
        }
    }
    
    public void OpenMenu()
    {
        gameObject.SetActive(true);
        RefreshMenu();
    }
    
    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }
    
    public void RefreshMenu()
    {
        ClearAllButtons();
        
        if (teaMaker == null)
        {
            Debug.LogError("TeaMaker reference is not set!");
            ShowErrorMessage("Ошибка: TeaMaker не назначен");
            return;
        }
        
        if (teaMaker.allTeas == null || teaMaker.allTeas.Count == 0)
        {
            Debug.LogWarning("No teas found in TeaMaker!");
            ShowErrorMessage("Нет доступных рецептов чая");
            return;
        }
        
        CreateTeaButtons();
        
        // Показываем первый чай по умолчанию
        if (teaMaker.allTeas.Count > 0)
        {
            ShowTeaDetails(teaMaker.allTeas[0]);
        }
    }
    
    private void ShowErrorMessage(string message)
    {
        if (selectedTeaName != null)
            selectedTeaName.text = message;
        
        if (selectedTeaDescription != null)
            selectedTeaDescription.text = "";
        
        ClearPanel(recipeIngredientsPanel);
        ClearPanel(likedSpiritsPanel);
    }
    
    private void CreateTeaButtons()
    {
        foreach (var tea in teaMaker.allTeas)
        {
            if (tea == null) continue;
            
            // Создаем кнопку
            GameObject buttonObj = Instantiate(teaButtonPrefab, teaListParent);
            teaButtons.Add(buttonObj);
            
            // Настраиваем кнопку
            SetupTeaButton(buttonObj, tea);
            
            // Назначаем обработчик клика
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                TeaData currentTea = tea;
                button.onClick.AddListener(() => ShowTeaDetails(currentTea));
            }
        }
    }
    
    private void SetupTeaButton(GameObject buttonObj, TeaData tea)
    {
        // Ищем все Image компоненты на кнопке
        Image[] images = buttonObj.GetComponentsInChildren<Image>();
        
        foreach (var image in images)
        {
            // Если это дочерний объект (не сам Button)
            if (image.transform.parent == buttonObj.transform)
            {
                // Устанавливаем иконку чая
                if (tea.icon != null)
                {
                    image.sprite = tea.icon;
                    image.preserveAspect = true;
                }
                break; // Используем первый дочерний Image
            }
        }
        
        // Если не нашли дочерний Image, используем основной
        if (buttonObj.GetComponent<Image>() != null && tea.icon != null)
        {
            buttonObj.GetComponent<Image>().sprite = tea.icon;
        }
        
        // Добавляем текст с названием если есть TextMeshPro
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = tea.teaName;
        }
    }
    
    private void ShowTeaDetails(TeaData tea)
    {
        if (tea == null) return;
        
        // Основная информация о чае
        if (selectedTeaIcon != null)
        {
            if (tea.icon != null)
            {
                selectedTeaIcon.sprite = tea.icon;
                selectedTeaIcon.preserveAspect = true;
            }
        }
        
        if (selectedTeaName != null)
        {
            selectedTeaName.text = tea.teaName;
        }
        
        if (selectedTeaDescription != null)
        {
            selectedTeaDescription.text = tea.description;
        }
        
        // Очищаем панели
        ClearPanel(recipeIngredientsPanel);
        ClearPanel(likedSpiritsPanel);
        
        // Отображаем ингредиенты рецепта
        if (tea.ingredients != null && tea.ingredients.Count > 0)
        {
            foreach (var ingredient in tea.ingredients)
            {
                if (ingredient == null) continue;
                
                CreateIconInPanel(recipeIngredientsPanel, ingredient.icon, ingredient.ingredientName);
            }
        }
        else
        {
            CreateTextInPanel(recipeIngredientsPanel, "Рецепт не указан");
        }
        
        // Отображаем духов
        if (tea.likedBySpirits != null && tea.likedBySpirits.Count > 0)
        {
            foreach (var spirit in tea.likedBySpirits)
            {
                if (spirit == null) continue;
                
                CreateIconInPanel(likedSpiritsPanel, spirit.icon, spirit.spiritName);
            }
        }
        else
        {
            CreateTextInPanel(likedSpiritsPanel, "Никому не нравится");
        }
    }
    
    private void CreateIconInPanel(Transform panel, Sprite icon, string name)
    {
        if (panel == null || iconPrefab == null) return;
        
        GameObject iconObj = Instantiate(iconPrefab, panel);
        
        // Настройка Image
        Image iconImage = iconObj.GetComponent<Image>();
        if (iconImage != null && icon != null)
        {
            iconImage.sprite = icon;
            iconImage.preserveAspect = true;
        }
        
        // Можно добавить подпись под иконкой
        TextMeshProUGUI text = iconObj.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = name;
            text.fontSize = 10;
        }
    }
    
    private void CreateTextInPanel(Transform panel, string text)
    {
        if (panel == null) return;
        
        GameObject textObj = new GameObject("InfoText");
        textObj.transform.SetParent(panel);
        
        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = 14;
        textComponent.color = Color.gray;
    }
    
    private void ClearPanel(Transform panel)
    {
        if (panel == null) return;
        
        foreach (Transform child in panel)
        {
            Destroy(child.gameObject);
        }
    }
    
    private void ClearAllButtons()
    {
        foreach (var button in teaButtons)
        {
            if (button != null)
                Destroy(button);
        }
        teaButtons.Clear();
    }
}