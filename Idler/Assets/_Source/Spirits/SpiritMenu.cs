using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpiritMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpiritCollection spiritCollection;
    
    [Header("UI Elements")]
    [SerializeField] private Transform spiritListParent; // Родитель для списка духов
    [SerializeField] private GameObject spiritButtonPrefab; // Префаб кнопки духа
    [SerializeField] private Image selectedSpiritIcon; // Большая иконка выбранного духа
    [SerializeField] private TextMeshProUGUI selectedSpiritName; // Имя духа
    [SerializeField] private TextMeshProUGUI selectedSpiritDescription; // Описание
    [SerializeField] private TextMeshProUGUI selectedSpiritBuff; // Информация о бусте
    [SerializeField] private Transform likedTeasPanel; // Панель для чаев, которые нравится духу
    [SerializeField] private GameObject teaIconPrefab; // Префаб для иконок чая
    
    [Header("Settings")]
    [SerializeField] private bool startHidden = true;
    
    private List<GameObject> spiritButtons = new List<GameObject>();
    
    private void Start()
    {
        if (startHidden)
        {
            gameObject.SetActive(false);
        }
    }
    
    // Открытие меню
    public void OpenMenu()
    {
        gameObject.SetActive(true);
        RefreshMenu();
    }
    
    // Закрытие меню
    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }
    
    // Обновление меню
    public void RefreshMenu()
    {
        ClearAllButtons();
        
        if (spiritCollection == null)
        {
            Debug.LogError("SpiritCollection reference is not set!");
            ShowErrorMessage("Ошибка: Коллекция духов не назначена");
            return;
        }
        
        CreateSpiritButtons();
        
        // Показываем первый дух по умолчанию
        if (spiritCollection.unlockedSpirits.Count > 0)
        {
            ShowSpiritDetails(spiritCollection.unlockedSpirits[0]);
        }
        else if (spiritCollection.allSpirits.Count > 0)
        {
            // Показываем первый дух из всех, даже если не разблокирован
            ShowSpiritDetails(spiritCollection.allSpirits[0]);
        }
    }
    
    private void ShowErrorMessage(string message)
    {
        if (selectedSpiritName != null)
            selectedSpiritName.text = message;
        
        if (selectedSpiritDescription != null)
            selectedSpiritDescription.text = "";
        
        if (selectedSpiritBuff != null)
            selectedSpiritBuff.text = "";
        
        ClearPanel(likedTeasPanel);
    }
    
    private void CreateSpiritButtons()
    {
        if (spiritCollection.allSpirits == null || spiritCollection.allSpirits.Count == 0)
        {
            Debug.LogWarning("No spirits found in SpiritCollection!");
            return;
        }
        
        foreach (var spirit in spiritCollection.allSpirits)
        {
            if (spirit == null) continue;
            
            // Создаем кнопку
            GameObject buttonObj = Instantiate(spiritButtonPrefab, spiritListParent);
            spiritButtons.Add(buttonObj);
            
            // Настраиваем кнопку
            SetupSpiritButton(buttonObj, spirit);
            
            // Назначаем обработчик клика
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                SpiritData currentSpirit = spirit;
                button.onClick.AddListener(() => ShowSpiritDetails(currentSpirit));
                
                // Если дух не разблокирован, кнопка неактивна или серая
                if (!spirit.isUnlocked)
                {
                    button.interactable = false;
                    
                    // Делаем иконку полупрозрачной
                    Image buttonImage = buttonObj.GetComponent<Image>();
                    if (buttonImage != null)
                    {
                        buttonImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                    }
                }
            }
        }
    }
    
    private void SetupSpiritButton(GameObject buttonObj, SpiritData spirit)
    {
        // Ищем Image для иконки духа
        Image[] images = buttonObj.GetComponentsInChildren<Image>();
        
        foreach (var image in images)
        {
            // Используем дочерний Image (не сам Button)
            if (image.transform.parent == buttonObj.transform)
            {
                if (spirit.icon != null)
                {
                    image.sprite = spirit.icon;
                    image.preserveAspect = true;
                    
                    // Если дух не разблокирован, делаем иконку темнее
                    if (!spirit.isUnlocked)
                    {
                        image.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                    }
                }
                break;
            }
        }
        
        // Если не нашли дочерний Image, используем основной
        if (buttonObj.GetComponent<Image>() != null && spirit.icon != null)
        {
            buttonObj.GetComponent<Image>().sprite = spirit.icon;
            
            if (!spirit.isUnlocked)
            {
                buttonObj.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
        }
        
        // Добавляем текст с именем духа
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = spirit.spiritName;
            
            // Добавляем замок если дух не разблокирован
            if (!spirit.isUnlocked)
            {
                buttonText.text += " 🔒";
            }
        }
    }
    
    private void ShowSpiritDetails(SpiritData spirit)
    {
        if (spirit == null) return;
        
        Debug.Log($"Showing details for spirit: {spirit.spiritName}");
        
        // Основная информация о духе
        if (selectedSpiritIcon != null)
        {
            if (spirit.icon != null)
            {
                selectedSpiritIcon.sprite = spirit.icon;
                selectedSpiritIcon.preserveAspect = true;
                
                // Если дух не разблокирован, делаем иконку темнее
                selectedSpiritIcon.color = spirit.isUnlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
        }
        
        if (selectedSpiritName != null)
        {
            selectedSpiritName.text = spirit.spiritName;
            if (!spirit.isUnlocked)
            {
                selectedSpiritName.text += " (Заблокирован)";
            }
        }
        
        if (selectedSpiritDescription != null)
        {
            selectedSpiritDescription.text = spirit.description;
        }
        
        if (selectedSpiritBuff != null)
        {
            if (spirit.isUnlocked)
            {
                selectedSpiritBuff.text = $"Буст: {spirit.buffName}\n{spirit.buffDescription}\nМножитель: {spirit.buffMultiplier}x";
            }
            else
            {
                selectedSpiritBuff.text = "Этот дух еще не найден";
            }
        }
        
        // Очищаем панель чаев
        ClearPanel(likedTeasPanel);
        
        // Отображаем чаи, которые нравятся духу
        if (spirit.likedTeas != null && spirit.likedTeas.Length > 0)
        {
            foreach (var tea in spirit.likedTeas)
            {
                if (tea == null) continue;
                
                CreateTeaIconInPanel(likedTeasPanel, tea);
            }
        }
        else
        {
            CreateTextInPanel(likedTeasPanel, "Не любит никакие чаи");
        }
    }
    
    private void CreateTeaIconInPanel(Transform panel, TeaData tea)
    {
        if (panel == null || teaIconPrefab == null) return;
        
        GameObject iconObj = Instantiate(teaIconPrefab, panel);
        
        // Настройка иконки чая
        Image iconImage = iconObj.GetComponent<Image>();
        if (iconImage != null && tea.icon != null)
        {
            iconImage.sprite = tea.icon;
            iconImage.preserveAspect = true;
        }
        
        // Добавляем подпись с названием чая
        TextMeshProUGUI text = iconObj.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = tea.teaName;
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
        foreach (var button in spiritButtons)
        {
            if (button != null)
                Destroy(button);
        }
        spiritButtons.Clear();
    }
    
    private void OnDestroy()
    {
        ClearAllButtons();
    }
}