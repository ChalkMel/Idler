// ZoneSelectionUI.cs - исправленный
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ZoneSelectionUI : MonoBehaviour
{
    [Header("Zone Buttons")]
    [SerializeField] private List<ZoneButton> _zoneButtons = new List<ZoneButton>();
    
    [Header("Selection Panel")]
    [SerializeField] private GameObject _explorationPanel;
    [SerializeField] private TextMeshProUGUI _zoneNameText;
    [SerializeField] private TextMeshProUGUI _zoneDescriptionText;
    [SerializeField] private Image _zoneIconImage;
    [SerializeField] private TextMeshProUGUI _zoneInfoText;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;
    
    [Header("Message")]
    [SerializeField] private TextMeshProUGUI _messageText;
    
    private ZoneUnlockService _unlockService;
    private ZoneData _selectedZone;
    private Coroutine _messageCoroutine;
    
    public event System.Action<ZoneData> OnZoneSelected;
    public event System.Action OnExplorationConfirmed;
    public event System.Action OnExplorationCancelled;
    
    private void Start()
    {
        _unlockService = GetComponent<ZoneUnlockService>();
        
        RefreshZoneButtonsList();
        
        foreach (var button in _zoneButtons)
        {
            if (button != null && button.ZoneData != null)
            {
                RegisterZoneButton(button, button.ZoneData);
            }
        }
        
        if (_confirmButton != null)
            _confirmButton.onClick.AddListener(() => OnExplorationConfirmed?.Invoke());
        
        if (_cancelButton != null)
            _cancelButton.onClick.AddListener(CancelSelection);
        
        if (_explorationPanel != null)
            _explorationPanel.SetActive(false);
    }
    
    private void RefreshZoneButtonsList()
    {
        ZoneButton[] foundButtons = FindObjectsByType<ZoneButton>(FindObjectsSortMode.None);
        _zoneButtons.Clear();
        _zoneButtons.AddRange(foundButtons);
    }

    private void RegisterZoneButton(ZoneButton button, ZoneData zone)
    {
        if (button == null) return;
        
        if (!_zoneButtons.Contains(button))
            _zoneButtons.Add(button);
        
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
            ShowMessage($"Все духи в {zone.zoneName} уже найдены!");
            return;
        }
        
        if (_zoneNameText != null)
            _zoneNameText.text = zone.zoneName;
        
        if (_zoneDescriptionText != null)
            _zoneDescriptionText.text = zone.zoneDescription;
        
        if (_zoneIconImage != null && zone.zoneIcon != null)
            _zoneIconImage.sprite = zone.zoneIcon;
        
        if (_zoneInfoText != null && _unlockService != null)
        {
            string spiritsInfo = _unlockService.GetZoneProgressText(zone);
            int cost = GetComponent<ExplorationExecutor>()?.GetExplorationCost(zone) ?? 20;
            _zoneInfoText.text = $"Время: {zone.explorationTime} сек\n{spiritsInfo}\nЦена: {cost} капель";
        }
        
        if (_explorationPanel != null)
            _explorationPanel.SetActive(true);
        
        OnZoneSelected?.Invoke(zone);
    }

    private void CancelSelection()
    {
        _selectedZone = null;
        if (_explorationPanel != null)
            _explorationPanel.SetActive(false);
        
        OnExplorationCancelled?.Invoke();
        
        RefreshAllButtonsState();
    }
    
    public void RefreshAllButtonsState()
    {
        RefreshZoneButtonsList();
    }
    
    public void UpdateCostDisplay(int cost)
    {
        if (_zoneInfoText != null && _selectedZone != null)
        {
            string spiritsInfo = _unlockService?.GetZoneProgressText(_selectedZone) ?? "";
            _zoneInfoText.text = $"Время: {_selectedZone.explorationTime} сек\n{spiritsInfo}\nЦена: {cost} капель";
        }
    }
    
    public ZoneData GetSelectedZone()
    {
        return _selectedZone;
    }
    
    public void ShowMessage(string message)
    {
        if (_messageText != null)
        {
            _messageText.text = message;
            
            if (_messageCoroutine != null)
                StopCoroutine(_messageCoroutine);
            _messageCoroutine = StartCoroutine(ClearMessageAfterDelay(3f));
        }
    }
    
    private System.Collections.IEnumerator ClearMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_messageText != null)
            _messageText.text = "";
    }
    
    public void ClosePanel()
    {
        if (_explorationPanel != null)
            _explorationPanel.SetActive(false);
        _selectedZone = null;
        
        RefreshAllButtonsState();
    }
    
    private void OnEnable()
    {
        RefreshAllButtonsState();
    }
}