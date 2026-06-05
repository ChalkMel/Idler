using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private Credits credits;
    [SerializeField] private SpiritCollection spiritCollection;
    [SerializeField] private SpiritBuffManager buffManager;
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private ExplorationExecutor explorationExecutor;
    [SerializeField] private TeaBrewingController teaMaker;
    [SerializeField] private TutorialManager tutorialManager;
    
    [Header("Shop Items for Save")]
    [SerializeField] private HelperItem helperItem;
    [SerializeField] private HelperDropletItem dropletHelperItem;
    [SerializeField] private DropletMultiplierItem dropletMultiplierItem;
    
    private const string SAVE_KEY = "GameSave";
    
    private void Start()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("No save - starting fresh");
        }
        else
        {
            LoadGame();
        }
    }
    
   public void SaveGame()
    {
        SaveData data = new SaveData();
        
        SaveCredits(data);
        SaveSpiritCollection(data);
        SaveShopItems(data);
        SaveChairUpgrade(data);
        SaveActiveBuffs(data);
        SaveHelpers(data);
        
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }
    
    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            return;
        }
        
        string json = PlayerPrefs.GetString(SAVE_KEY);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        
        LoadCredits(data);
        LoadSpiritCollection(data);
        LoadShopItems(data);
        LoadChairUpgrade(data);
        LoadActiveBuffs(data);
        LoadHelpers(data);
    }
    
    private void SaveCredits(SaveData data)
    {
        data.droplets = credits.droplets;
        data.leaves = credits.leaves;
        data.berries = credits.berries;
        data.flowers = credits.flowers;
        data.dropletsMulti = credits.dropletsMulti;
        data.leavesMulti = credits.leavesMulti;
        data.berriesMulti = credits.berriesMulti;
        data.flowersMulti = credits.flowersMulti;
        data.helperCount = credits.HelperCount;
        data.dropletHelperCount = credits.DropletHelperCount;
    }
    
    private void LoadCredits(SaveData data)
    {
        credits.droplets = data.droplets;
        credits.leaves = data.leaves;
        credits.berries = data.berries;
        credits.flowers = data.flowers;
        credits.dropletsMulti = data.dropletsMulti;
        credits.leavesMulti = data.leavesMulti;
        credits.berriesMulti = data.berriesMulti;
        credits.flowersMulti = data.flowersMulti;
        credits.HelperCount = data.helperCount;
        credits.DropletHelperCount = data.dropletHelperCount;
        credits.UpdateUI();
    }
    
     private void SaveHelpers(SaveData data)
    {
        if (helperItem != null)
        {
            data.helperCurrentCost = helperItem.cost;
            data.helperBaseCost = helperItem.baseCost;
            data.helperCostMultiplier = helperItem.costMultiplier;
            data.helperBoughtCount = helperItem.boughtCount;
        }
        
        if (dropletHelperItem != null)
        {
            data.dropletHelperCurrentCost = dropletHelperItem.cost;
            data.dropletHelperBaseCost = dropletHelperItem.baseCost;
            data.dropletHelperCostMultiplier = dropletHelperItem.costMultiplier;
            data.dropletHelperBoughtCount = dropletHelperItem.boughtCount;
        }
        
        if (dropletMultiplierItem != null)
        {
            data.dropletMultiplierCurrentCost = dropletMultiplierItem.cost;
            data.dropletMultiplierBaseCost = dropletMultiplierItem.baseCost;
            data.dropletMultiplierCostMultiplier = dropletMultiplierItem.costMultiplier;
            data.dropletMultiplierBoughtCount = dropletMultiplierItem.boughtCount;
            data.dropletMultiplierTotalValue = dropletMultiplierItem.totalMultiplierValue;
        }
    }
    
    private void LoadHelpers(SaveData data)
    {
        if (helperItem != null)
        {
            helperItem.baseCost = data.helperBaseCost > 0 ? data.helperBaseCost : 50;
            helperItem.costMultiplier = data.helperCostMultiplier > 0 ? data.helperCostMultiplier : 2;
            helperItem.boughtCount = data.helperBoughtCount;
            helperItem.UpdateCost();
        }
        
        if (dropletHelperItem != null)
        {
            dropletHelperItem.baseCost = data.dropletHelperBaseCost > 0 ? data.dropletHelperBaseCost : 50;
            dropletHelperItem.costMultiplier = data.dropletHelperCostMultiplier > 0 ? data.dropletHelperCostMultiplier : 2;
            dropletHelperItem.boughtCount = data.dropletHelperBoughtCount;
            dropletHelperItem.UpdateCost();
        }
        
        if (dropletMultiplierItem != null)
        {
            dropletMultiplierItem.baseCost = data.dropletMultiplierBaseCost > 0 ? data.dropletMultiplierBaseCost : 50;
            dropletMultiplierItem.costMultiplier = data.dropletMultiplierCostMultiplier > 0 ? data.dropletMultiplierCostMultiplier : 2;
            dropletMultiplierItem.boughtCount = data.dropletMultiplierBoughtCount;
            dropletMultiplierItem.totalMultiplierValue = data.dropletMultiplierTotalValue;
            dropletMultiplierItem.UpdateCost();
        }
    }
    
    private void SaveTutorial(SaveData data)
    {
        if (tutorialManager != null)
            data.tutorialCompleted = tutorialManager.IsTutorialCompleted();
    }
    
    private void LoadTutorial(SaveData data)
    {
        if (tutorialManager != null && data.tutorialCompleted)
            tutorialManager.SetTutorialCompleted();
    }
    
    private void SaveSpiritCollection(SaveData data)
    {
        foreach (var spirit in spiritCollection.unlockedSpirits)
        {
            data.unlockedSpiritNames.Add(spirit.spiritName);
        }
    }
    
    private void LoadSpiritCollection(SaveData data)
    {
        spiritCollection.unlockedSpirits.Clear();
        spiritCollection.availableSpirits.Clear();
        
        foreach (var spiritName in data.unlockedSpiritNames)
        {
            SpiritData spirit = spiritCollection.allSpirits.Find(s => s.spiritName == spiritName);
            if (spirit != null)
            {
                spirit.isUnlocked = true;
                spiritCollection.unlockedSpirits.Add(spirit);
                spiritCollection.availableSpirits.Add(spirit);
            }
        }
    }
    
    private void SaveShopItems(SaveData data)
    {
        if (shopManager == null) return;
        
        foreach (var item in shopManager.GetAllShopItems())
        {
            if (item.isPurchased)
                data.purchasedShopItemNames.Add(item.itemName);
        }
    }
    
    private void LoadShopItems(SaveData data)
    {
        if (shopManager == null) return;
        
        foreach (var item in shopManager.GetAllShopItems())
        {
            item.isPurchased = data.purchasedShopItemNames.Contains(item.itemName);
        }
    }
    
    private void SaveChairUpgrade(SaveData data)
    {
        data.hasChairUpgrade = buffManager.MaxSpiritSlots >= 2;
        data.maxSpiritSlots = buffManager.MaxSpiritSlots;
    }
    
    private void LoadChairUpgrade(SaveData data)
    {
        if (data.hasChairUpgrade)
        {
            buffManager.SetUpChair();
        }
    }
    
    private void SaveActiveBuffs(SaveData data)
    {
        foreach (var activeSpirit in buffManager.ActiveSpirits)
        {
            SavedSpiritBuff saved = new SavedSpiritBuff
            {
                spiritName = activeSpirit.SpiritData.spiritName,
                endTime = activeSpirit.EndTime - Time.time,
                slotIndex = activeSpirit.SlotIndex,
                multiplier = activeSpirit.Multiplier
            };
            data.activeSpiritBuffs.Add(saved);
        }
    }
    
    private void LoadActiveBuffs(SaveData data)
    {
        foreach (var saved in data.activeSpiritBuffs)
        {
            SpiritData spirit = spiritCollection.allSpirits.Find(s => s.spiritName == saved.spiritName);
            if (spirit != null && saved.endTime > 0)
            {
                buffManager.LoadBuff(spirit, saved.endTime, saved.slotIndex, saved.multiplier);
            }
        }
    }
    
    public void DeleteSave()
    {
        Time.timeScale = 1;
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();
        tutorialManager.ResetTutorial();
        Debug.Log("Save deleted");
        SceneManager.LoadScene(1);
        Application.Quit();
    }
}