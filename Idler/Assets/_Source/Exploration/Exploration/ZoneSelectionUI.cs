using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ZoneSelectionUI : MonoBehaviour
{
    [Header("Zone Buttons")]
    [SerializeField] private List<ZoneButton> zoneButtons = new List<ZoneButton>();
    
   
    [Header("Selection Panel")]
    [SerializeField] private GameObject explorationPanel;
    [SerializeField] private TextMeshProUGUI zoneNameText;
    [SerializeField] private TextMeshProUGUI zoneDescriptionText;
    [SerializeField] private Image zoneIconImage;
    [SerializeField] private TextMeshProUGUI zoneInfoText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    
    [Header("Message")]
    [SerializeField] private TextMeshProUGUI messageText;
    
    private ZoneUnlockService _unlockService;
    private ZoneData _selectedZone;
    private Coroutine _messageCoroutine;
    
    public event System.Action OnExplorationConfirmed;
    public event System.Action OnExplorationCancelled;
    
    private void Start()
    {
        _unlockService = GetComponent<ZoneUnlockService>();
        
        RefreshZoneButtonsList();
        
        foreach (var button in zoneButtons)
        {
            if (button != null && button.ZoneData != null)
            {
                RegisterZoneButton(button, button.ZoneData);
            }
        }
        
        if (confirmButton != null)
            confirmButton.onClick.AddListener(() => OnExplorationConfirmed?.Invoke());
        
        if (cancelButton != null)
            cancelButton.onClick.AddListener(CancelSelection);
        
        if (explorationPanel != null)
            explorationPanel.SetActive(false);
    }
    
    private void RefreshZoneButtonsList()
    {
        ZoneButton[] foundButtons = FindObjectsByType<ZoneButton>(FindObjectsSortMode.None);
        zoneButtons.Clear();
        zoneButtons.AddRange(foundButtons);
    }

    private void RegisterZoneButton(ZoneButton button, ZoneData zone)
    {
        if (button == null) return;
        
        if (!zoneButtons.Contains(button))
            zoneButtons.Add(button);
        
        Button btnComponent = button.GetComponent<Button>();
        if (btnComponent != null)
        {
            btnComponent.onClick.RemoveAllListeners();
            btnComponent.onClick.AddListener(() => SelectZone(zone));
        }
    }
    
    public void SelectZone(ZoneData zone)
    {
        if (zone == null)
        {
            Debug.LogError("Zone is null!");
            return;
        }
        
        _selectedZone = zone;
        
        if (_unlockService != null && _unlockService.IsZoneComplete(zone))
        {
            ShowMessage($"Все духи в {zone.ZoneName} уже найдены!");
            return;
        }
        
        if (zoneNameText != null)
            zoneNameText.text = zone.ZoneName;
        
        if (zoneDescriptionText != null)
            zoneDescriptionText.text = zone.ZoneDescription;
        
        if (zoneIconImage != null && zone.ZoneIcon != null)
            zoneIconImage.sprite = zone.ZoneIcon;
        
        if (zoneInfoText != null && _unlockService != null)
        {
            string spiritsInfo = _unlockService.GetZoneProgressText(zone);
            int cost = GetComponent<ExplorationExecutor>()?.GetExplorationCost(zone) ?? 20;
            zoneInfoText.text = $"Время: {zone.ExplorationTime} сек\n{spiritsInfo}\nЦена: {cost} капель";
        }
        
        if (explorationPanel != null)
            explorationPanel.SetActive(true);
    }

    private void CancelSelection()
    {
        _selectedZone = null;
        if (explorationPanel != null)
            explorationPanel.SetActive(false);
        
        OnExplorationCancelled?.Invoke();
        
        RefreshAllButtonsState();
    }
    
    public void RefreshAllButtonsState()
    {
        RefreshZoneButtonsList();
    }
    
    public void UpdateCostDisplay(int cost)
    {
        if (zoneInfoText != null && _selectedZone != null)
        {
            string spiritsInfo = _unlockService?.GetZoneProgressText(_selectedZone) ?? "";
            zoneInfoText.text = $"Время: {_selectedZone.ExplorationTime} сек\n{spiritsInfo}\nЦена: {cost} капель";
        }
    }
    
    public ZoneData GetSelectedZone()
    {
        return _selectedZone;
    }
    
    public void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
            
            if (_messageCoroutine != null)
                StopCoroutine(_messageCoroutine);
            _messageCoroutine = StartCoroutine(ClearMessageAfterDelay(3f));
        }
    }
    
    private System.Collections.IEnumerator ClearMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (messageText != null)
            messageText.text = "";
    }
    
    public void ClosePanel()
    {
        if (explorationPanel != null)
            explorationPanel.SetActive(false);
        _selectedZone = null;
        
        RefreshAllButtonsState();
    }
    
    private void OnEnable()
    {
        RefreshAllButtonsState();
    }
}