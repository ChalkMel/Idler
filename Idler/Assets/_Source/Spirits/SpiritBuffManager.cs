using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpiritBuffManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform buffsPanel;
    [SerializeField] private GameObject buffUIPrefab;
    [SerializeField] private TextMeshProUGUI buffTimerText;
    [SerializeField] private Image activeBuffIcon;
    [SerializeField] private TextMeshProUGUI activeBuffNameText;
    [SerializeField] private Credits credits;
    
    private SpiritBuff currentBuff;
    
    private void Update()
    {
        UpdateBuff();
        UpdateUI();
    }
    
    public void AddBuff(SpiritData spirit)
    {
        ClearBuff();
        switch (spirit.spiritId)
        {
            case 0 :
                credits.dropletsMulti = 2;
                break;
            case 1 :
                credits.flowersMulti = 2;
                credits.berriesMulti = 2;
                break;
        }
        Debug.Log("here");
        currentBuff = new SpiritBuff
        {
            buffName = spirit.buffName,
            multiplier = spirit.buffMultiplier,
            duration = spirit.buffDuration,
            endTime = Time.time + spirit.buffDuration,
            spirit = spirit
        };
        
        UpdateBuffUI();
    }
    
    public void ClearBuff()
    {
        currentBuff = null;

        if (activeBuffIcon != null)
            activeBuffIcon.gameObject.SetActive(false);
        
        if (activeBuffNameText != null)
            activeBuffNameText.text = "";
        
        credits.dropletsMulti = 1;
        credits.flowersMulti = 1;
        credits.berriesMulti = 1;
        credits.leavesMulti = 1;

    }
    
    private void UpdateBuff()
    {
        if (currentBuff != null && !currentBuff.IsActive)
        {
            ClearBuff();
        }
    }
    
    private void UpdateUI()
    {
        if (buffTimerText != null)
        {
            if (currentBuff != null && currentBuff.IsActive)
            {
                float timeLeft = currentBuff.TimeLeft;
                buffTimerText.text = $"Буст: {Mathf.CeilToInt(timeLeft)}с";
            }
            else
            {
                buffTimerText.text = "Буста нет";
            }
        }
    }
    
    private void UpdateBuffUI()
    {
        if (currentBuff == null || currentBuff.spirit == null) return;
        
        if (activeBuffIcon != null && currentBuff.spirit.icon != null)
        {
            activeBuffIcon.sprite = currentBuff.spirit.icon;
            activeBuffIcon.gameObject.SetActive(true);
        }
        
        if (activeBuffNameText != null)
        {
            activeBuffNameText.text = $"{currentBuff.spirit.spiritName}\nx{currentBuff.multiplier:F1}";
        }
    }
    
    public float GetCurrentMultiplier()
    {
        return currentBuff != null && currentBuff.IsActive ? currentBuff.multiplier : 1f;
    }
    
    // Проверить, активен ли буст
    public bool HasActiveBuff()
    {
        return currentBuff != null && currentBuff.IsActive;
    }
    
    // Получить текущий дух
    public SpiritData GetCurrentSpirit()
    {
        return currentBuff != null && currentBuff.IsActive ? currentBuff.spirit : null;
    }
}