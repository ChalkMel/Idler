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
    
    private List<ShopItemButton> itemButtons = new List<ShopItemButton>();
    
    private void Start()
    {
        UpdateDropletsDisplay();
        
        InitializeShopItems();
        
        if (messageText != null)
            messageText.gameObject.SetActive(false);
            
        Debug.Log("ShopManager started. Found " + itemButtons.Count + " item buttons");
    }
    
    private void InitializeShopItems()
    {
        itemButtons.Clear();
        itemButtons.AddRange(GetComponentsInChildren<ShopItemButton>(true));
        
        Debug.Log("Found " + itemButtons.Count + " shop item buttons");
        
        for (int i = 0; i < itemButtons.Count; i++)
        {
            ShopItemButton button = itemButtons[i];
            if (button == null) continue;
            
            Button btnComponent = button.GetComponent<Button>();
            if (btnComponent != null)
            {
                btnComponent.onClick.RemoveAllListeners();
            }
            
            int index = i;
            btnComponent.onClick.AddListener(() => OnItemButtonClicked(index));

            UpdateItemButton(button);
            
            Debug.Log($"Set up button for item: {(button.shopItem != null ? button.shopItem.itemName : "NULL")}");
        }
    }
    
    private void OnItemButtonClicked(int buttonIndex)
    {
        Debug.Log($"Button clicked: {buttonIndex}");
        
        if (buttonIndex < 0 || buttonIndex >= itemButtons.Count)
        {
            Debug.LogError($"Invalid button index: {buttonIndex}");
            return;
        }
        
        ShopItemButton button = itemButtons[buttonIndex];
        if (button == null || button.shopItem == null)
        {
            Debug.LogError("Button or ShopItem is null!");
            return;
        }
        
        Debug.Log($"Trying to buy: {button.shopItem.itemName}");
        BuyItem(button.shopItem);
    }
    
    public void BuyItem(ShopItem item)
    {
        if (item == null)
        {
            ShowMessage("Ошибка: предмет не найден");
            Debug.LogError("Trying to buy null item!");
            return;
        }
        
        Debug.Log($"BuyItem called for: {item.itemName}, Purchased: {item.isPurchased}, Cost: {item.cost}, Player droplets: {credits.droplets}");
        
        if (item.isPurchased)
        {
            ShowMessage($"Уже куплено: {item.itemName}");
            return;
        }
        
        if (credits.droplets < item.cost)
        {
            ShowMessage($"Недостаточно капель! Нужно: {item.cost}");
            return;
        }

        credits.droplets -= item.cost;
        item.isPurchased = true;

        ApplyItemEffect(item);
        
        UpdateDropletsDisplay();
        UpdateAllItemButtons();
        
        ShowMessage($"Куплено: {item.itemName}!");
        Debug.Log($"Purchased: {item.itemName}");
    }
    
    private void ApplyItemEffect(ShopItem item)
    {
        switch (item.itemType)
        {
            case ShopItemType.CauldronUpgrade:
                Debug.Log($"Кастрюля улучшена: {item.effectValue}");
                break;
                
            case ShopItemType.BrewingSpeed:
                Debug.Log($"Скорость варки увеличена: {item.effectValue}");
                break;
                
            case ShopItemType.IngredientBoost:
                Debug.Log($"Буст ингредиентов: {item.effectValue}");
                break;
                
            case ShopItemType.SpiritChance:
                Debug.Log($"Шанс найти духа: {item.effectValue}");
                break;
                
            case ShopItemType.DropletMultiplier:
                Debug.Log($"Множитель капель: {item.effectValue}");
                break;
        }
    }
    
    private void UpdateItemButton(ShopItemButton button)
    {
        if (button == null || button.shopItem == null) return;
        
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            if (button.shopItem.isPurchased)
            {
                buttonText.text = "КУПЛЕНО";
            }
            else
            {
                buttonText.text = $"КУПИТЬ\n{button.shopItem.cost}";
            }
        }

        Button btn = button.GetComponent<Button>();
        if (btn != null)
        {
            btn.interactable = !button.shopItem.isPurchased && credits.droplets >= button.shopItem.cost;
        }

        if (button.itemPriceText != null)
        {
            if (button.shopItem.isPurchased)
            {
                button.itemPriceText.text = "КУПЛЕНО";
                button.itemPriceText.color = Color.green;
            }
            else
            {
                button.itemPriceText.text = $"{button.shopItem.cost} капель";
                button.itemPriceText.color = Color.white;
            }
        }
    }
    
    private void UpdateAllItemButtons()
    {
        foreach (var button in itemButtons)
        {
            UpdateItemButton(button);
        }
    }
    
    private void UpdateDropletsDisplay()
    {
        if (dropletsText != null && credits != null)
        {
            dropletsText.text = $"Капли: {credits.droplets}";
        }
    }
    
    private void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
            messageText.gameObject.SetActive(true);
            
            CancelInvoke(nameof(HideMessage));
            Invoke(nameof(HideMessage), messageDisplayTime);
        }
    }
    
    private void HideMessage()
    {
        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }
    
    public void OpenShop()
    {
        gameObject.SetActive(true);
        UpdateDropletsDisplay();
        UpdateAllItemButtons();
    }
    
    public void CloseShop()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (credits != null)
        {
            UpdateDropletsDisplay();
            UpdateAllItemButtons();
        }
    }
}