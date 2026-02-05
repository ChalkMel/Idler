using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpiritBuffManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject BuffPanel;
    [SerializeField] private TextMeshProUGUI buffTimerText;
    [SerializeField] private Image activeBuffIcon;
    [SerializeField] private Credits credits;
    
    [Header("Tooltip")]
    [SerializeField] private GameObject tooltipObject;
    [SerializeField] private TextMeshProUGUI tooltipText;
    
    private SpiritBuff _currentBuff;
    private bool _isHovering;
    
    private void Start()
    {
        if (tooltipObject != null)
            tooltipObject.SetActive(false);
    }
    
    private void Update()
    {
        UpdateBuff();
        UpdateUI();

        if (activeBuffIcon != null && activeBuffIcon.gameObject.activeSelf)
        {
            CheckMouseHover();
        }
    }
    
    private void CheckMouseHover()
    {
        RectTransform rect = activeBuffIcon.rectTransform;
        Vector2 mousePos = Input.mousePosition;

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect, 
            mousePos, 
            null, 
            out localPos
        );
        
        bool wasHovering = _isHovering;
        _isHovering = rect.rect.Contains(localPos);
        
        if (_isHovering && !wasHovering)
        {
            ShowTooltip();
        }
        else if (!_isHovering && wasHovering)
        {
            HideTooltip();
        }
    }
    
    public void AddBuff(SpiritData spirit)
    {
        ClearCurrentBuff();

        switch (spirit.spiritId)
        {
            case 0:
                credits.dropletsMulti += 1;
                break;
            case 1:
                credits.flowersMulti += 1;
                credits.berriesMulti += 1;
                break;
        }
        
        _currentBuff = new SpiritBuff
        {
            buffName = spirit.buffName,
            multiplier = spirit.buffMultiplier,
            duration = spirit.buffDuration,
            endTime = Time.time + spirit.buffDuration,
            spirit = spirit
        };
        
        if (activeBuffIcon != null && spirit.icon != null)
        {
            activeBuffIcon.sprite = spirit.icon;
            activeBuffIcon.gameObject.SetActive(true);
        }
    }

    private void ClearCurrentBuff()
    {
        if (_currentBuff == null) return;

        if (_currentBuff.spirit != null)
        {
            switch (_currentBuff.spirit.spiritId)
            {
                case 0:
                    credits.dropletsMulti -= 1; 
                    break;
                case 1:
                    credits.flowersMulti -= 1;
                    credits.berriesMulti -= 1;
                    break;
            }
        }
    }
    
    private void ShowTooltip()
    {
        if (tooltipObject == null || tooltipText == null) return;
        if (_currentBuff == null || _currentBuff.spirit == null) return;
        
        tooltipText.text = $"{_currentBuff.spirit.spiritName}\n{_currentBuff.spirit.buffName}";
        tooltipObject.SetActive(true);

        if (activeBuffIcon != null)
        {
            Vector3 iconPos = activeBuffIcon.transform.position;
            tooltipObject.transform.position = new Vector3(iconPos.x, iconPos.y + 50f, iconPos.z);
        }
    }
    
    private void HideTooltip()
    {
        if (tooltipObject != null)
            tooltipObject.SetActive(false);
        _isHovering = false;
    }
    
    private void ClearBuff()
    {
        if (_currentBuff == null) return;
        if (_currentBuff.spirit != null)
        {
            switch (_currentBuff.spirit.spiritId)
            {
                case 0:
                    credits.dropletsMulti -= 1;
                    break;
                case 1:
                    credits.flowersMulti -= 1;
                    credits.berriesMulti -= 1;
                    break;
            }
        }
        
        _currentBuff = null;
        
        if (activeBuffIcon != null)
            activeBuffIcon.gameObject.SetActive(false);
        
        HideTooltip();
    }
    
    private void UpdateBuff()
    {
        if (_currentBuff != null && !_currentBuff.IsActive)
        {
            ClearBuff();
        }
    }
    
    private void UpdateUI()
    {
        if (buffTimerText != null)
        {
            if (_currentBuff != null && _currentBuff.IsActive)
            {
                BuffPanel.SetActive(true);
                float timeLeft = _currentBuff.TimeLeft;
                buffTimerText.text = $"Boost: {Mathf.CeilToInt(timeLeft)}s";
            }
            else
            {
                BuffPanel.SetActive(false);
                buffTimerText.text = "No Boost";
            }
        }
    }
    
    public bool HasActiveBuff()
    {
        return _currentBuff != null && _currentBuff.IsActive;
    }
}