using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int droplets;
    public int leaves;
    public int berries;
    public int flowers;
    
    public float dropletsMulti;
    public float leavesMulti;
    public float berriesMulti;
    public float flowersMulti;
    
    public int helperCount;
    
    public List<string> unlockedSpiritNames = new List<string>();
    public List<string> purchasedShopItemNames = new List<string>();
    
    public bool hasChairUpgrade;
    public int maxSpiritSlots;
    
    public List<SavedSpiritBuff> activeSpiritBuffs = new List<SavedSpiritBuff>();
    
    public List<SavedExploration> explorations = new List<SavedExploration>();
}

[Serializable]
public class SavedSpiritBuff
{
    public string spiritName;
    public float endTime;
    public int slotIndex;
    public float multiplier;
}

[Serializable]
public class SavedExploration
{
    public string zoneName;
    public float timeRemaining;
    public bool isExploring;
}
