using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Credits credits;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI dropletsText;
    [SerializeField] private TextMeshProUGUI messageText;
    
    [Header("Shop Items")]
    [SerializeField] private List<ShopItem> shopItems = new List<ShopItem>();
    
    [Header("Settings")]
    [SerializeField] private float messageDisplayTime = 2f;
    
    private List<ShopItemButton> _itemButtons = new List<ShopItemButton>();
    
    private void Start()
    {
        UpdateDropletsDisplay();
        
        InitializeShopItems();
        
        messageText.gameObject.SetActive(false);
        foreach (var item in shopItems)
        {
            item.isPurchased = false;
            if(item.itemType == ShopItemType.Helper)
                item.cost = 200;
        }
        
    }
    
    private void InitializeShopItems()
    {
        _itemButtons.Clear();
        _itemButtons.AddRange(GetComponentsInChildren<ShopItemButton>(true));
        
        
        for (int i = 0; i < _itemButtons.Count; i++)
        {
            ShopItemButton button = _itemButtons[i];
            if (button == null) continue;
            
            Button btnComponent = button.GetComponent<Button>();
            if (btnComponent != null)
            {
                btnComponent.onClick.RemoveAllListeners();
            }
            
            int index = i;
            btnComponent.onClick.AddListener(() => OnItemButtonClicked(index));

            UpdateItemButton(button);
        }
    }
    
    private void OnItemButtonClicked(int buttonIndex)
    {
        ShopItemButton button = _itemButtons[buttonIndex];
        BuyItem(button.shopItem);
    }

    private void BuyItem(ShopItem item)
    {
        if (item.isPurchased)
        {
            ShowMessage($"Already bought: {item.itemName}");
            return;
        }
        
        if (credits.droplets < item.cost)
        {
            ShowMessage($"Not enough! You need: {item.cost}");
            return;
        }
        item.isPurchased = true;
        credits.droplets -= item.cost;
        item.ApplyEffect(item, credits);

        UpdateDropletsDisplay();
        UpdateAllItemButtons();
    }
    
    private void UpdateItemButton(ShopItemButton button)
    {
        if (button == null || button.shopItem == null) return;
        
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (button.shopItem.isPurchased)
        {
            buttonText.text = $"{button.shopItem.itemName}";
        }
        else
        {
            buttonText.text = $"Buy\n{button.shopItem.cost}";
        }

        Button btn = button.GetComponent<Button>();
        btn.interactable = !button.shopItem.isPurchased && credits.droplets >= button.shopItem.cost;

        if (button.shopItem.isPurchased)
        {
            button.itemPriceText.text = "Bought";
            button.itemPriceText.color = Color.green;
        }
        else
        {
            button.itemPriceText.text = $"{button.shopItem.cost} droplets";
            button.itemPriceText.color = Color.white;
        }
    }
    
    private void UpdateAllItemButtons()
    {
        foreach (var button in _itemButtons)
        {
            UpdateItemButton(button);
        }
    }
    
    private void UpdateDropletsDisplay()
    {
        dropletsText.text = $"{credits.droplets} of droplets";
    }
    
    private void ShowMessage(string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);
            
        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), messageDisplayTime);
        
    }
    
    private void HideMessage()
    {
        messageText.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        UpdateDropletsDisplay();
        UpdateAllItemButtons();
    }
}