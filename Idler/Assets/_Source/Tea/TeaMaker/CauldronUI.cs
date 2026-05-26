using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CauldronUI : MonoBehaviour
{
    [Header("Ingredient Icons")]
    [SerializeField] private Transform _ingredientsPanel;
    [SerializeField] private GameObject _ingredientUIPrefab;

    [Header("Counters")]
    [SerializeField] private TextMeshProUGUI _berryCountText;
    [SerializeField] private TextMeshProUGUI _flowerCountText;
    [SerializeField] private TextMeshProUGUI _leafCountText;

    [Header("Buttons")]
    [SerializeField] private Button _brewButton;
    [SerializeField] private Button _clearButton;

    [Header("Message")]
    [SerializeField] private TextMeshProUGUI _messageText;

    private List<GameObject> _ingredientIcons = new List<GameObject>();

    public Button BrewButton => _brewButton;
    public Button ClearButton => _clearButton;

    public void AddIngredientIcon(IngredientData ingredient)
    {
        if (_ingredientUIPrefab == null || _ingredientsPanel == null) return;
        GameObject iconObj = Instantiate(_ingredientUIPrefab, _ingredientsPanel);
        Image img = iconObj.GetComponentInChildren<Image>();
        if (img != null && ingredient.icon != null) img.sprite = ingredient.icon;
        _ingredientIcons.Add(iconObj);
        
        Debug.Log($"Added icon, total icons: {_ingredientIcons.Count}");
    }

    public void ClearIngredientIcons()
    {
        Debug.Log($"Clearing {_ingredientIcons.Count} icons");
        
        foreach (var icon in _ingredientIcons)
        {
            if (icon != null) 
                Destroy(icon);
        }
        _ingredientIcons.Clear();
        
        if (_ingredientsPanel != null)
        {
            foreach (Transform child in _ingredientsPanel)
            {
                if (child != null)
                    Destroy(child.gameObject);
            }
        }
    }

    public void UpdateCounters(Credits credits)
    {
        if (credits == null) return;
        if (_berryCountText != null) _berryCountText.text = credits.berries.ToString();
        if (_flowerCountText != null) _flowerCountText.text = credits.flowers.ToString();
        if (_leafCountText != null) _leafCountText.text = credits.leaves.ToString();
    }

    public void SetBrewButtonInteractable(bool interactable)
    {
        if (_brewButton != null) 
            _brewButton.interactable = interactable;
    }

    public void SetClearButtonInteractable(bool interactable)
    {
        if (_clearButton != null) 
            _clearButton.interactable = interactable;
    }

    public void ShowMessage(string message)
    {
        if (_messageText != null) 
            _messageText.text = message;
    }
}