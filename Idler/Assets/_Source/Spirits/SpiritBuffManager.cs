using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpiritBuffManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform buffsPanel;
    [SerializeField] private GameObject buffUIPrefab;
    [SerializeField] private TextMeshProUGUI buffTimerText;
    
    private List<SpiritBuff> activeBuffs = new List<SpiritBuff>();
    
    private void Update()
    {
        UpdateBuffs();
        UpdateUI();
    }
    
    public void AddBuff(SpiritData spirit)
    {
        foreach (var buff in activeBuffs)
        {
            if (buff.spirit == spirit)
            {
                buff.endTime = Time.time + buff.duration;
                return;
            }
        }
        
        SpiritBuff newBuff = new SpiritBuff
        {
            buffName = spirit.buffName,
            multiplier = spirit.buffMultiplier,
            duration = 30f, // 30 секунд по умолчанию, можно вынести в настройки
            endTime = Time.time + 30f,
            spirit = spirit
        };
        
        activeBuffs.Add(newBuff);
        
        // Создаем UI элемент
        CreateBuffUI(newBuff);
    }
    
    private void UpdateBuffs()
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            if (!activeBuffs[i].IsActive)
            {
                activeBuffs.RemoveAt(i);
            }
        }
    }
    
    private void UpdateUI()
    {
        if (buffTimerText != null)
        {
            if (activeBuffs.Count > 0)
            {
                // Показываем самый короткий таймер
                float minTime = float.MaxValue;
                foreach (var buff in activeBuffs)
                {
                    if (buff.TimeLeft < minTime)
                        minTime = buff.TimeLeft;
                }
                buffTimerText.text = $"Бусты: {Mathf.CeilToInt(minTime)}с";
            }
            else
            {
                buffTimerText.text = "Бустов нет";
            }
        }
    }
    
    private void CreateBuffUI(SpiritBuff buff)
    {
        if (buffsPanel == null || buffUIPrefab == null) return;
        
        GameObject buffUI = Instantiate(buffUIPrefab, buffsPanel);
        
        BuffUIElement uiElement = buffUI.GetComponent<BuffUIElement>();
        if (uiElement != null)
        {
            uiElement.Setup(buff);
        }
    }
    
    public float GetTotalMultiplier()
    {
        float total = 1f;
        foreach (var buff in activeBuffs)
        {
            if (buff.IsActive)
            {
                total *= buff.multiplier;
            }
        }
        return total;
    }
    
    public bool IsBuffActive(SpiritData spirit)
    {
        foreach (var buff in activeBuffs)
        {
            if (buff.spirit == spirit && buff.IsActive)
                return true;
        }
        return false;
    }
}