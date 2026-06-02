using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpiritBuffManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI buffTimerText;
    [SerializeField] private Credits credits;
    [SerializeField] private SpiritCollection spiritCollection;
    [SerializeField] private GameObject heartImage;
    
    [Header("Spirit Slots")]
    [SerializeField] private Image[] spiritSlots;
    [SerializeField] private Image[] chairSlots;
    [SerializeField] public int MaxSpiritSlots;

    public List<ActiveSpirit> ActiveSpirits = new List<ActiveSpirit>(); 
    
    private void Start()
    {
        if (heartImage != null)
            heartImage.SetActive(false);
    }
    
    public void SetUpChair()
    {
        chairSlots[1].gameObject.SetActive(true);
        MaxSpiritSlots = 2;
    }

    public class ActiveSpirit
    {
        public SpiritData SpiritData;
        public float EndTime;
        public int SlotIndex;
        public float Multiplier;
        
        public bool IsActive => Time.time < EndTime;
        public float TimeLeft => Mathf.Max(0, EndTime - Time.time);
    }
    
    private void Update()
    {
        UpdateBuffs();
    }
    
    public void AddBuff(SpiritData spirit)
    {
        if (ActiveSpirits.Count >= MaxSpiritSlots)
        {
            Debug.Log("All spirit slots are full!");
            return;
        }
        
        foreach (var activeSpirit in ActiveSpirits)
        {
            RelationshipType relationship = spirit.GetRelationshipWith(activeSpirit.SpiritData);
            
            if (relationship == RelationshipType.Negative)
            {
                Debug.Log($"{spirit.spiritName} doesn't get along with {activeSpirit.SpiritData.spiritName}!");
                return;
            }
        }
        
        List<ActiveSpirit> spiritsToRemove = new List<ActiveSpirit>();
        foreach (var activeSpirit in ActiveSpirits)
        {
            RelationshipType relationship = activeSpirit.SpiritData.GetRelationshipWith(spirit);
            if (relationship == RelationshipType.Negative)
            {
                Debug.Log($"{activeSpirit.SpiritData.spiritName} leaves because of {spirit.spiritName}!");
                spiritsToRemove.Add(activeSpirit);
            }
        }
        
        foreach (var spiritToRemove in spiritsToRemove)
        {
            RemoveSpirit(spiritToRemove);
        }
        
        if (ActiveSpirits.Count >= MaxSpiritSlots)
        {
            Debug.Log("No space after negative reaction!");
            return;
        }
        
        int freeSlotIndex = GetFreeSlotIndex();
        if (freeSlotIndex == -1)
        {
            Debug.Log("No free slot!");
            return;
        }

        float finalMultiplier = spirit.buffMultiplier;
        bool hasPositive = false;
        
        foreach (var activeSpirit in ActiveSpirits)
        {
            RelationshipType relationship = spirit.GetRelationshipWith(activeSpirit.SpiritData);
            if (relationship == RelationshipType.Positive)
            {
                finalMultiplier *= 2;
                hasPositive = true;
                Debug.Log($"Positive relationship! {spirit.spiritName} buff doubled!");
            }
            
            relationship = activeSpirit.SpiritData.GetRelationshipWith(spirit);
            if (relationship == RelationshipType.Positive)
            {
                float oldMultiplier = activeSpirit.Multiplier;
                activeSpirit.Multiplier = activeSpirit.SpiritData.buffMultiplier * 2;
                
                ApplySpiritEffect(activeSpirit.SpiritData, false, oldMultiplier);
                ApplySpiritEffect(activeSpirit.SpiritData, true, activeSpirit.Multiplier);
                Debug.Log($"Positive relationship! {activeSpirit.SpiritData.spiritName} buff doubled!");
            }
        }
        
        ApplySpiritEffect(spirit, true, finalMultiplier);
    
        ActiveSpirit activeSpiritNew = new ActiveSpirit
        {
            SpiritData = spirit,
            EndTime = Time.time + spirit.buffDuration,
            SlotIndex = freeSlotIndex, 
            Multiplier = finalMultiplier
        };
    
        ActiveSpirits.Add(activeSpiritNew);
    
        if (freeSlotIndex < spiritSlots.Length)
        {
            spiritSlots[freeSlotIndex].sprite = spirit.icon;
            spiritSlots[freeSlotIndex].gameObject.SetActive(true);
    
            SpiritSlot slotComponent = spiritSlots[freeSlotIndex].GetComponent<SpiritSlot>();
            if (slotComponent != null)
            {
                slotComponent.SetSpiritData(spirit.spiritName, spirit.buffName);
            }
        }
        
        if (hasPositive || CheckAnyPositiveRelationship())
        {
            if (heartImage != null)
                heartImage.SetActive(true);
        }
    }
    
    private bool CheckAnyPositiveRelationship()
    {
        for (int i = 0; i < ActiveSpirits.Count; i++)
        {
            for (int j = i + 1; j < ActiveSpirits.Count; j++)
            {
                if (ActiveSpirits[i].SpiritData.GetRelationshipWith(ActiveSpirits[j].SpiritData) == RelationshipType.Positive)
                    return true;
                if (ActiveSpirits[j].SpiritData.GetRelationshipWith(ActiveSpirits[i].SpiritData) == RelationshipType.Positive)
                    return true;
            }
        }
        return false;
    }
    
    private void RemoveSpirit(ActiveSpirit spirit)
    {
        ApplySpiritEffect(spirit.SpiritData, false, spirit.Multiplier);
    
        spiritCollection.availableSpirits.Add(spirit.SpiritData);
        
        int slotIndex = spirit.SlotIndex;
        if (slotIndex < spiritSlots.Length && spiritSlots[slotIndex] != null)
        {
            spiritSlots[slotIndex].gameObject.SetActive(false);
        
            SpiritSlot slotComponent = spiritSlots[slotIndex].GetComponent<SpiritSlot>();
            if (slotComponent != null)
            {
                slotComponent.ClearSpiritData();
            }
        }
    
        ActiveSpirits.Remove(spirit);
        
        if (!CheckAnyPositiveRelationship())
        {
            if (heartImage != null)
                heartImage.SetActive(false);
        }
    }
    
    private int GetFreeSlotIndex()
    {
        for (int i = 0; i < spiritSlots.Length; i++)
        {
            bool slotOccupied = false;
            foreach (var spirit in ActiveSpirits)
            {
                if (spirit.SlotIndex == i)
                {
                    slotOccupied = true;
                    break;
                }
            }
            if (!slotOccupied)
            {
                return i;
            }
        }
        return -1;
    }
    
    private void ApplySpiritEffect(SpiritData spirit, bool apply, float multiplier)
    {
        spiritCollection.availableSpirits.Remove(spirit);
        if (spirit.effect == null)
        {
            Debug.LogWarning($"Spirit {spirit.spiritName} has no effect assigned!");
            return;
        }
        
        if (apply)
            spirit.effect.Apply(credits, this, multiplier);
        else
            spirit.effect.Remove(credits, this, multiplier);
    }
    
    private void UpdateBuffs()
    {
        for (int i = ActiveSpirits.Count - 1; i >= 0; i--)
        {
            if (!ActiveSpirits[i].IsActive)
            {
                RemoveSpirit(ActiveSpirits[i]);
            }
        }
    }

    public bool IsAlreadyHere(SpiritData spirit)
    {
        foreach (var activeSpirit in ActiveSpirits)
        {
            if (activeSpirit.SpiritData == spirit)
                return true;
        }
        return false;
    }
    
    public bool HasActiveBuff()
    {
        return ActiveSpirits.Count > 0;
    }

    #region Gets

    public int GetActiveSpiritsCount()
    {
        return ActiveSpirits.Count;
    }
    
    public float GetTimeLeftForSlot(int slotID)
    {
        foreach (var spirit in ActiveSpirits)
        {
            if (spirit.SlotIndex == slotID)
            {
                return spirit.TimeLeft;
            }
        }
        
        return 0f;
    }

    public string GetSpiritNameForSlot(int slotID)
    {
        foreach (var spirit in ActiveSpirits)
        {
            if (spirit.SlotIndex == slotID)
                return spirit.SpiritData.spiritName;
        }
        return "";
    }

    public string GetBuffNameForSlot(int slotID)
    {
        foreach (var spirit in ActiveSpirits)
        {
            if (spirit.SlotIndex == slotID)
                return spirit.SpiritData.buffName;
        }
        return "";
    }

    #endregion

    #region Save

    public void LoadBuff(SpiritData spirit, float remainingTime, int slotIndex, float multiplier)
    {
        ActiveSpirit activeSpirit = new ActiveSpirit
        {
            SpiritData = spirit,
            EndTime = Time.time + remainingTime,
            SlotIndex = slotIndex,
            Multiplier = multiplier
        };
    
        ActiveSpirits.Add(activeSpirit);
    
        if (slotIndex < spiritSlots.Length)
        {
            spiritSlots[slotIndex].sprite = spirit.icon;
            spiritSlots[slotIndex].gameObject.SetActive(true);
        
            SpiritSlot slotComponent = spiritSlots[slotIndex].GetComponent<SpiritSlot>();
            if (slotComponent != null)
            {
                slotComponent.SetSpiritData(spirit.spiritName, spirit.buffName);
            }
        }
    
        ApplySpiritEffect(spirit, true, multiplier);
    }

    #endregion
}