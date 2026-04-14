using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class SpiritSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Settings")]
    [SerializeField]
    public int slotID = -1;
    
    [Header("Tooltip")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI spiritNameText;
    [SerializeField] private TextMeshProUGUI buffNameText;
    [SerializeField] private TextMeshProUGUI timeLeftText;
    
    private SpiritBuffManager _buffManager;
    private string _spiritName;
    private string _buffName;
    
    private void Start()
    {
        _buffManager = FindFirstObjectByType<SpiritBuffManager>();
        
        if (slotID == -1)
        {
            Debug.LogError($"Slot {gameObject.name} has no slotID assigned!");
        }
    }
    
    public void SetSpiritData(string spiritName, string buffName)
    {
        _spiritName = spiritName;
        _buffName = buffName;
    }
    
    public void ClearSpiritData()
    {
        _spiritName = "";
        _buffName = "";
    }
    
    private Coroutine _tooltipUpdateCoroutine;

    private void UpdateTooltipData()
    {
        if (_buffManager == null) return;
    
        _buffManager.GetTimeLeftForSlot(slotID);
    
        if (spiritNameText != null)
            spiritNameText.text = _spiritName;
    
        if (buffNameText != null)
            buffNameText.text = _buffName;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipPanel != null)
        {
            UpdateTooltipData();
            tooltipPanel.SetActive(true);
            
            if (_tooltipUpdateCoroutine != null)
                StopCoroutine(_tooltipUpdateCoroutine);
            _tooltipUpdateCoroutine = StartCoroutine(UpdateTimeCoroutine());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipUpdateCoroutine != null)
            StopCoroutine(_tooltipUpdateCoroutine);
        
        tooltipPanel.SetActive(false);
    }

    private IEnumerator UpdateTimeCoroutine()
    {
        while (tooltipPanel.activeSelf)
        {
            float timeLeft = _buffManager.GetTimeLeftForSlot(slotID);
        
            if (timeLeft <= 0)
                timeLeftText.text = "0s";
            else if (timeLeft >= 60)
                timeLeftText.text = $"{Mathf.FloorToInt(timeLeft / 60)}:{Mathf.FloorToInt(timeLeft % 60):00}";
            else
                timeLeftText.text = $"{Mathf.CeilToInt(timeLeft)}s";
        
            yield return new WaitForSeconds(1f);
        }
    }
}