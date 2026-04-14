using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpiritBuffManager : MonoBehaviour
{
    [Header("UI")]
    //[SerializeField] private GameObject BuffPanel;
    [SerializeField] private TextMeshProUGUI buffTimerText;
    [SerializeField] private Credits credits;
    [SerializeField] private SpiritCollection spiritCollection;
    
    [Header("Spirit Slots")]
    [SerializeField] private Image[] spiritSlots;
    [SerializeField] private Image[] chairSlots;
    [SerializeField] public int MaxSpiritSlots;

    public List<ActiveSpirit> ActiveSpirits = new List<ActiveSpirit>(); 
    
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
        
        foreach (var activeSpirit in ActiveSpirits)
        {
            RelationshipType relationship = spirit.GetRelationshipWith(activeSpirit.SpiritData);
            if (relationship == RelationshipType.Positive)
            {
                finalMultiplier *= 2;
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
        int multiplierValue = apply ? 1 : -1;
        spiritCollection.availableSpirits.Remove(spirit);
        switch (spirit.spiritId)
        {
            case 0:
                credits.dropletsMulti += multiplierValue * multiplier;
                break;
            case 1:
                credits.flowersMulti += multiplierValue * multiplier;
                credits.berriesMulti += multiplierValue * multiplier;
                break;
            case 3:
                var maker = FindFirstObjectByType<TeaMaker>();
                foreach (var tea in maker.allTeas)
                {
                    tea.brewingTime /= multiplier * multiplierValue;
                }
                break;
        }
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
}