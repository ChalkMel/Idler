using System;
using UnityEngine;
[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Shop Item/Chair")]
public class ChairItem : ShopItem
{
    public override void ApplyEffect(ShopItem item, Credits credits, SpiritBuffManager spiritBuffManager)
    {
        base.ApplyEffect(item, credits, spiritBuffManager);
        isPurchased = true;
        spiritBuffManager.SetUpChair();
    }
}
