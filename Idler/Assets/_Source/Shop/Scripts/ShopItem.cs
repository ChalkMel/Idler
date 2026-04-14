using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Shop Item")]
public abstract class ShopItem : ScriptableObject
{
  [Header("Basic Info")]
  public string itemName;
  public Sprite icon;
  [TextArea(2, 4)] public string description;
    
  [Header("Purchase")]
  public int cost;
  public bool isPurchased = false;

  public virtual void ApplyEffect(ShopItem item, Credits credits, SpiritBuffManager spiritBuffManager)
  {
    
  }
}