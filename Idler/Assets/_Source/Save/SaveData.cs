
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
  public int dropletHelperCount;
  
  public int helperCurrentCost;
  public int helperBaseCost;
  public int helperCostMultiplier;
  public int helperBoughtCount;
    
  public int dropletHelperCurrentCost;
  public int dropletHelperBaseCost;
  public int dropletHelperCostMultiplier;
  public int dropletHelperBoughtCount;
    
  // Прогрессия цены тучки (DropletMultiplier)
  public int dropletMultiplierCurrentCost;
  public int dropletMultiplierBaseCost;
  public int dropletMultiplierCostMultiplier;
  public int dropletMultiplierBoughtCount;
  public float dropletMultiplierTotalValue;
    
  public List<string> unlockedSpiritNames = new List<string>();
  public List<string> purchasedShopItemNames = new List<string>();
    
  public bool hasChairUpgrade;
  public int maxSpiritSlots;
    
  public List<SavedSpiritBuff> activeSpiritBuffs = new List<SavedSpiritBuff>();
    
  public bool tutorialCompleted;
}

[Serializable]
public class SavedSpiritBuff
{
  public string spiritName;
  public float endTime;
  public int slotIndex;
  public float multiplier;
}

