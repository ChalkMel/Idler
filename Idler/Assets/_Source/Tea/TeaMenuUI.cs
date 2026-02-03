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
    [SerializeField] private Image selectedIcon;
    [SerializeField] private TextMeshProUGUI selectedName;
    [SerializeField] private TextMeshProUGUI selectedDescription;
    [SerializeField] private Transform ingredientsPanel;
    [SerializeField] private Transform spiritsPanel;
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
            return;
        }
        
        if (teaMaker.allTeas == null || teaMaker.allTeas.Count == 0)
        {
            Debug.LogWarning("No teas found in TeaMaker!");
            ShowEmptyMenu();
            return;
        }
        
        CreateTeaButtons();
        ShowTeaDetails(teaMaker.allTeas[0]);
    }
    
    private void ShowEmptyMenu()
    {
        if (selectedName != null)
            selectedName.text = "Нет чаев";
        
        if (selectedDescription != null)
            selectedDescription.text = "Добавьте рецепты чаев в TeaMaker";
        
        ClearPanel(ingredientsPanel);
        ClearPanel(spiritsPanel);
    }
    
    private void CreateTeaButtons()
    {
        foreach (var tea in teaMaker.allTeas)
        {
            if (tea == null) continue;
            
            GameObject buttonObj = Instantiate(teaButtonPrefab, teaListParent);
            teaButtons.Add(buttonObj);
            Image buttonImage = null;
            Image bgImage = buttonObj.GetComponent<Image>();
            
            Image[] childImages = buttonObj.GetComponentsInChildren<Image>();
            foreach (var img in childImages)
            {
                if (img.gameObject == buttonObj) continue;
                
                buttonImage = img;
                break;
            }
            
            if (buttonImage == null)
                buttonImage = bgImage;
            
            if (buttonImage != null && tea.icon != null)
            {
                buttonImage.sprite = tea.icon;
                buttonImage.preserveAspect = true;
            }
            
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = tea.teaName;
            }
            
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                TeaData currentTea = tea;
                button.onClick.AddListener(() => ShowTeaDetails(currentTea));
            }
        }
    }
    
    private void ShowTeaDetails(TeaData tea)
    {
        if (tea == null) return;
        
        if (selectedIcon != null)
        {
            selectedIcon.sprite = tea.icon;
            selectedIcon.preserveAspect = true;
            Debug.Log($"Set icon: {tea.icon?.name}");
        }
        
        if (selectedName != null)
        {
            selectedName.text = tea.teaName;
            Debug.Log($"Set name: {tea.teaName}");
        }
        
        if (selectedDescription != null)
        {
            selectedDescription.text = tea.description;
        }
        
        ClearPanel(ingredientsPanel);
        ClearPanel(spiritsPanel);
        
        if (tea.ingredients != null && tea.ingredients.Count > 0)
        {
            foreach (var ingredient in tea.ingredients)
            {
                if (ingredient == null) continue;
                
                GameObject iconObj = Instantiate(iconPrefab, ingredientsPanel);
                Image iconImage = iconObj.GetComponent<Image>();
                if (iconImage != null && ingredient.icon != null)
                {
                    iconImage.sprite = ingredient.icon;
                    iconImage.preserveAspect = true;
                }
            }
        }
        
        if (tea.likedBySpirits != null && tea.likedBySpirits.Count > 0)
        {
            foreach (var spirit in tea.likedBySpirits)
            {
                GameObject iconObj = Instantiate(iconPrefab, spiritsPanel);
                Image iconImage = iconObj.GetComponent<Image>();
                iconImage.sprite = spirit;
                iconImage.preserveAspect = true;
                
            }
        }
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