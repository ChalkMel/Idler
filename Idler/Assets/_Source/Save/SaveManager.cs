using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private Credits credits;
    [SerializeField] private SpiritCollection spiritCollection;
    [SerializeField] private SpiritBuffManager buffManager;
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private ExplorationExecutor explorationExecutor;
    [SerializeField] private TeaBrewingController teaMaker;
    
    private const string SAVE_KEY = "GameSave";
    private const string FIRST_LAUNCH_KEY = "FirstLaunch";
    
    private void Start()
    {
        if (IsFirstLaunch())
        {
            Debug.Log("First launch - creating new save");
            PlayerPrefs.SetInt(FIRST_LAUNCH_KEY, 1);
            PlayerPrefs.Save();
        }
        else
        {
            LoadGame();
        }
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SaveGame();
    }
    
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    
    private bool IsFirstLaunch()
    {
        return !PlayerPrefs.HasKey(FIRST_LAUNCH_KEY);
    }
    
    public void SaveGame()
    {
        SaveData data = new SaveData();
        
        SaveCredits(data);
        SaveSpiritCollection(data);
        SaveShopItems(data);
        SaveChairUpgrade(data);
        SaveActiveBuffs(data);
        SaveExplorations(data);
        
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
        
        Debug.Log("Game saved!");
    }
    
    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("No save found");
            return;
        }
        
        string json = PlayerPrefs.GetString(SAVE_KEY);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        
        LoadCredits(data);
        LoadSpiritCollection(data);
        LoadShopItems(data);
        LoadChairUpgrade(data);
        LoadActiveBuffs(data);
        LoadExplorations(data);
        
        Debug.Log("Game loaded!");
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
        credits.UpdateUI();
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
            if (spirit != null)
            {
                float remainingTime = saved.endTime;
                if (remainingTime > 0)
                {
                    buffManager.LoadBuff(spirit, remainingTime, saved.slotIndex, saved.multiplier);
                }
            }
        }
    }
    
    private void SaveExplorations(SaveData data)
    {
        if (explorationExecutor == null) return;
        
        var currentExploration = explorationExecutor.GetCurrentExploration();
        if (currentExploration != null)
        {
            SavedExploration saved = new SavedExploration
            {
                zoneName = currentExploration.zone.zoneName,
                timeRemaining = currentExploration.timeRemaining,
                isExploring = currentExploration.isExploring
            };
            data.explorations.Add(saved);
        }
    }
    
    private void LoadExplorations(SaveData data)
    {
        if (explorationExecutor == null || data.explorations.Count == 0) return;
        
        var saved = data.explorations[0];
        ZoneData zone = FindZoneByName(saved.zoneName);
        if (zone != null && saved.isExploring)
        {
            explorationExecutor.LoadExploration(zone, saved.timeRemaining);
        }
    }
    
    private ZoneData FindZoneByName(string zoneName)
    {
        return Resources.LoadAll<ZoneData>("").FirstOrDefault(z => z.zoneName == zoneName);
    }
    
    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.DeleteKey(FIRST_LAUNCH_KEY);
        PlayerPrefs.Save();
        Debug.Log("Save deleted");
    }
    
    public bool HasSave()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }
}