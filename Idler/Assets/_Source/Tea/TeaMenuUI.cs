using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeaMenu : MonoBehaviour
{
    [SerializeField] private TeaCollection teaCollection;
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
    
    private List<GameObject> _teaButtons = new List<GameObject>();
    
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

    private void RefreshMenu()
    {
        ClearAllButtons();
        
        CreateTeaButtons();

        if (teaCollection.allTeas.Count > 0)
        {
            ShowTeaDetails(teaCollection.allTeas[0]);
        }
    }
    
    private void CreateTeaButtons()
    {
        foreach (var tea in teaCollection.allTeas)
        {
            if (tea == null) continue;

            GameObject buttonObj = Instantiate(teaButtonPrefab, teaListParent);
            _teaButtons.Add(buttonObj);

            SetupTeaButton(buttonObj, tea);

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
        Image[] images = buttonObj.GetComponentsInChildren<Image>();
        
        foreach (var image in images)
        {
            if (image.transform.parent == buttonObj.transform)
            {

                image.sprite = tea.Icon;
                image.preserveAspect = true;
                
                break;
            }
        }

        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = tea.TeaName;
        
    }
    
    private void ShowTeaDetails(TeaData tea)
    {
        if (tea == null) return;

        if (selectedTeaIcon != null)
        {
            if (tea.Icon != null)
            {
                selectedTeaIcon.sprite = tea.Icon;
                selectedTeaIcon.preserveAspect = true;
            }
        }
        
        if (selectedTeaName != null)
        {
            selectedTeaName.text = tea.TeaName;
        }
        
        if (selectedTeaDescription != null)
        {
            selectedTeaDescription.text = tea.Description;
        }

        ClearPanel(recipeIngredientsPanel);
        ClearPanel(likedSpiritsPanel);

        if (tea.Ingredients is {Count: > 0})
        {
            foreach (var ingredient in tea.Ingredients)
            {
                if (ingredient == null) continue;
                
                CreateIconInPanel(recipeIngredientsPanel, ingredient.Icon, ingredient.IngredientName);
            }
        }

        if (tea.LikedBySpirits is {Count: > 0})
        {
            foreach (var spirit in tea.LikedBySpirits)
            {
                if (spirit == null) continue;
                
                CreateIconInPanel(likedSpiritsPanel, spirit.Icon, spirit.SpiritName);
            }
        }
    }
    
    private void CreateIconInPanel(Transform panel, Sprite icon, string name)
    {
        if (panel == null || iconPrefab == null) return;
        
        GameObject iconObj = Instantiate(iconPrefab, panel);

        Image iconImage = iconObj.GetComponent<Image>();
        if (iconImage != null && icon != null)
        {
            iconImage.sprite = icon;
            iconImage.preserveAspect = true;
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
        foreach (var button in _teaButtons)
        {
            if (button != null)
                Destroy(button);
        }
        _teaButtons.Clear();
    }
}