// ActiveBuffManager.cs
using System.Collections.Generic;
using UnityEngine;

public class ActiveBuffManager : MonoBehaviour
{
    public static ActiveBuffManager Instance { get; private set; }
    
    private List<SpiritBuff> activeBuffs = new List<SpiritBuff>();
    private float globalMultiplier = 1.0f;
    
    public event System.Action OnBuffsChanged;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Update()
    {
        // Удаляем истекшие бусты
        bool removed = false;
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            if (!activeBuffs[i].IsActive)
            {
                activeBuffs.RemoveAt(i);
                removed = true;
            }
        }
        
        if (removed)
        {
            RecalculateGlobalMultiplier();
            OnBuffsChanged?.Invoke();
        }
    }
    
    // Добавление нового буста
    public void AddBuff(SpiritData spirit)
    {
        // Проверяем, есть ли уже такой буст
        foreach (var buff in activeBuffs)
        {
            if (buff.spirit == spirit)
            {
                // Обновляем время существующего буста
                buff.endTime = Time.time + spirit.buffDuration;
                RecalculateGlobalMultiplier();
                OnBuffsChanged?.Invoke();
                return;
            }
        }
        
        // Создаем новый буст
        SpiritBuff newBuff = new SpiritBuff
        {
            buffName = spirit.buffName,
            multiplier = spirit.buffMultiplier,
            duration = spirit.buffDuration,
            endTime = Time.time + spirit.buffDuration,
            spirit = spirit
        };
        
        activeBuffs.Add(newBuff);
        RecalculateGlobalMultiplier();
        OnBuffsChanged?.Invoke();
        
        Debug.Log($"Added buff from {spirit.spiritName} for {spirit.buffDuration} seconds");
    }
    
    // Перерасчет общего множителя
    private void RecalculateGlobalMultiplier()
    {
        globalMultiplier = 1.0f;
        
        foreach (var buff in activeBuffs)
        {
            if (buff.IsActive)
            {
                globalMultiplier *= buff.multiplier;
            }
        }
        
        Debug.Log($"Global multiplier updated: {globalMultiplier:F2}x");
    }
    
    // Получение общего множителя
    public float GetGlobalMultiplier()
    {
        return globalMultiplier;
    }
    
    // Получение списка активных бустов
    public List<SpiritBuff> GetActiveBuffs()
    {
        return new List<SpiritBuff>(activeBuffs);
    }
    
    // Получение времени до окончания конкретного буста
    public float GetBuffTimeLeft(SpiritData spirit)
    {
        foreach (var buff in activeBuffs)
        {
            if (buff.spirit == spirit && buff.IsActive)
            {
                return buff.TimeLeft;
            }
        }
        return 0f;
    }
    
    // Удаление всех бустов
    public void ClearAllBuffs()
    {
        activeBuffs.Clear();
        globalMultiplier = 1.0f;
        OnBuffsChanged?.Invoke();
    }
}