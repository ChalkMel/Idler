using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Shop Item")]
public class ShopItem : ScriptableObject
{
  [Header("Basic Info")]
  public string itemName;
  public Sprite icon;
  [TextArea(2, 4)] public string description;
    
  [Header("Purchase")]
  public int cost = 100;
  public bool isPurchased;
    
  [Header("Effect")]
  public ShopItemType itemType;
  public int effectValue = 1;

  public void ApplyEffect(ShopItem item, Credits credits)
  {
    switch (item.itemType)
    {
      case ShopItemType.DropletMultiplier:
        credits.dropletsMulti += effectValue;
        Debug.Log("here");
        break;
      case ShopItemType.IngredientBoost:
        credits.leavesMulti += effectValue;
        credits.berriesMulti += effectValue;
        credits.flowersMulti += effectValue;
        break;
      case ShopItemType.Helper:
        credits.HelperCount++;
        item.isPurchased = false;
        item.cost *= 2;
        break;
      case ShopItemType.CauldronUpgrade:
        foreach (TeaData tea in FindFirstObjectByType<TeaMaker>().allTeas)
        {
          tea.brewingTime /= item.effectValue;
        }
        break;
    }
  }
}

public enum ShopItemType
{
  CauldronUpgrade,
  IngredientBoost,    
  DropletMultiplier,
  Helper
}