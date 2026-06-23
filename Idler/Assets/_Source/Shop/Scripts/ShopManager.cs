using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Credits credits;
    [SerializeField] private SpiritBuffManager spiritBuffManager;
    
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
        BuyItem(button.ShopItem);
        credits.UpdateUI();
    }

    private void BuyItem(ShopItem item)
    {
        if (item.isPurchased)
        {
            ShowMessage($"Уже купили: {item.ItemName}");
            return;
        }
        
        if (credits.droplets < item.Cost)
        {
            ShowMessage($"Недостаточно: {item.Cost}");
            return;
        }
        item.isPurchased = true;
        credits.droplets -= item.Cost;
        item.ApplyEffect(item, credits, spiritBuffManager);

        UpdateDropletsDisplay();
        UpdateAllItemButtons();
    }
    
    private void UpdateItemButton(ShopItemButton button)
    {
        if (button == null || button.ShopItem == null) return;
        
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = $"{button.ShopItem.ItemName}";
        Button btn = button.GetComponent<Button>();
        btn.interactable = !button.ShopItem.isPurchased && credits.droplets >= button.ShopItem.Cost;

        if (button.ShopItem.isPurchased)
        {
            button.ItemPriceText.text = "куплено";
            button.ItemPriceText.color = Color.green;
        }
        else
        {
            button.ItemPriceText.text = $"{button.ShopItem.Cost}";
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
        dropletsText.text = $"{credits.droplets}";
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

    #region Save

    public List<ShopItem> GetAllShopItems()
    {
        return shopItems;
    }
    
    public void RefreshUI()
    {
        UpdateAllItemButtons();
    }

    #endregion
}