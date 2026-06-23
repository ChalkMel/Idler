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
        data.dropletsMulti = credits.DropletsMulti;
        data.leavesMulti = credits.LeavesMulti;
        data.berriesMulti = credits.BerriesMulti;
        data.flowersMulti = credits.FlowersMulti;
        data.helperCount = credits.HelperCount;
        data.dropletHelperCount = credits.DropletHelperCount;
    }
    
    private void LoadCredits(SaveData data)
    {
        credits.droplets = data.droplets;
        credits.leaves = data.leaves;
        credits.berries = data.berries;
        credits.flowers = data.flowers;
        credits.DropletsMulti = data.dropletsMulti;
        credits.LeavesMulti = data.leavesMulti;
        credits.BerriesMulti = data.berriesMulti;
        credits.FlowersMulti = data.flowersMulti;
        credits.HelperCount = data.helperCount;
        credits.DropletHelperCount = data.dropletHelperCount;
        credits.UpdateUI();
    }
    
     private void SaveHelpers(SaveData data)
    {
        if (helperItem != null)
        {
            data.helperCurrentCost = helperItem.Cost;
            data.helperBaseCost = helperItem.BaseCost;
            data.helperCostMultiplier = helperItem.CostMultiplier;
            data.helperBoughtCount = helperItem.BoughtCount;
        }
        
        if (dropletHelperItem != null)
        {
            data.dropletHelperCurrentCost = dropletHelperItem.Cost;
            data.dropletHelperBaseCost = dropletHelperItem.BaseCost;
            data.dropletHelperCostMultiplier = dropletHelperItem.CostMultiplier;
            data.dropletHelperBoughtCount = dropletHelperItem.BoughtCount;
        }
        
        if (dropletMultiplierItem != null)
        {
            data.dropletMultiplierCurrentCost = dropletMultiplierItem.Cost;
            data.dropletMultiplierBaseCost = dropletMultiplierItem.BaseCost;
            data.dropletMultiplierCostMultiplier = dropletMultiplierItem.CostMultiplier;
            data.dropletMultiplierBoughtCount = dropletMultiplierItem.BoughtCount;
            data.dropletMultiplierTotalValue = dropletMultiplierItem.TotalMultiplierValue;
        }
    }
    
    private void LoadHelpers(SaveData data)
    {
        if (helperItem != null)
        {
            helperItem.BaseCost = data.helperBaseCost > 0 ? data.helperBaseCost : 50;
            helperItem.CostMultiplier = data.helperCostMultiplier > 0 ? data.helperCostMultiplier : 2;
            helperItem.BoughtCount = data.helperBoughtCount;
            helperItem.UpdateCost();
        }
        
        if (dropletHelperItem != null)
        {
            dropletHelperItem.BaseCost = data.dropletHelperBaseCost > 0 ? data.dropletHelperBaseCost : 50;
            dropletHelperItem.CostMultiplier = data.dropletHelperCostMultiplier > 0 ? data.dropletHelperCostMultiplier : 2;
            dropletHelperItem.BoughtCount = data.dropletHelperBoughtCount;
            dropletHelperItem.UpdateCost();
        }
        
        if (dropletMultiplierItem != null)
        {
            dropletMultiplierItem.BaseCost = data.dropletMultiplierBaseCost > 0 ? data.dropletMultiplierBaseCost : 50;
            dropletMultiplierItem.CostMultiplier = data.dropletMultiplierCostMultiplier > 0 ? data.dropletMultiplierCostMultiplier : 2;
            dropletMultiplierItem.BoughtCount = data.dropletMultiplierBoughtCount;
            dropletMultiplierItem.TotalMultiplierValue = data.dropletMultiplierTotalValue;
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
        foreach (var spirit in spiritCollection.UnlockedSpirits)
        {
            data.unlockedSpiritNames.Add(spirit.SpiritName);
        }
    }
    
    private void LoadSpiritCollection(SaveData data)
    {
        spiritCollection.UnlockedSpirits.Clear();
        spiritCollection.AvailableSpirits.Clear();
        
        foreach (var spiritName in data.unlockedSpiritNames)
        {
            SpiritData spirit = spiritCollection.AllSpirits.Find(s => s.SpiritName == spiritName);
            if (spirit != null)
            {
                spirit.isUnlocked = true;
                spiritCollection.UnlockedSpirits.Add(spirit);
                spiritCollection.AvailableSpirits.Add(spirit);
            }
        }
    }
    
    private void SaveShopItems(SaveData data)
    {
        if (shopManager == null) return;
        
        foreach (var item in shopManager.GetAllShopItems())
        {
            if (item.isPurchased)
                data.purchasedShopItemNames.Add(item.ItemName);
        }
    }
    
    private void LoadShopItems(SaveData data)
    {
        if (shopManager == null) return;
        
        foreach (var item in shopManager.GetAllShopItems())
        {
            item.isPurchased = data.purchasedShopItemNames.Contains(item.ItemName);
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
                spiritName = activeSpirit.SpiritData.SpiritName,
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
            SpiritData spirit = spiritCollection.AllSpirits.Find(s => s.SpiritName == saved.spiritName);
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