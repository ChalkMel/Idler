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
    
    public event System.Action<ZoneData> OnZoneSelected;
    public event System.Action OnExplorationConfirmed;
    public event System.Action OnExplorationCancelled;
    
    private void Start()
    {
        _unlockService = GetComponent<ZoneUnlockService>();
        
        if (_zoneButtons == null || _zoneButtons.Count == 0)
        {
            ZoneButton[] foundButtons = FindObjectsByType<ZoneButton>(FindObjectsSortMode.None);
            _zoneButtons.AddRange(foundButtons);
            Debug.Log($"Found {_zoneButtons.Count} zone buttons");
        }
        
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
    
    public void RegisterZoneButton(ZoneButton button, ZoneData zone)
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
        
        Debug.Log($"Registered zone button: {zone.zoneName}");
    }
    
    public void SelectZone(ZoneData zone)
    {
        Debug.Log($"ZoneSelectionUI.SelectZone called for: {zone.zoneName}");
        
        if (zone == null)
        {
            Debug.LogError("Zone is null!");
            return;
        }
        
        _selectedZone = zone;
        
        if (_unlockService != null && _unlockService.IsZoneComplete(zone))
        {
            ShowMessage($"All spirits in {zone.zoneName} already found!");
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
            _zoneInfoText.text = $"Time: {zone.explorationTime} sec\n{spiritsInfo}";
        }
        
        if (_explorationPanel != null)
            _explorationPanel.SetActive(true);
        
        OnZoneSelected?.Invoke(zone);
    }
    
    public void CancelSelection()
    {
        _selectedZone = null;
        if (_explorationPanel != null)
            _explorationPanel.SetActive(false);
        
        OnExplorationCancelled?.Invoke();
    }
    
    public void UpdateZoneButtonVisual(ZoneData zone, bool isComplete, bool isUnlocked, bool isExploring)
    {
        foreach (var button in _zoneButtons)
        {
            if (button != null && button.ZoneData == zone)
            {
                button.UpdateVisual(isComplete, isUnlocked, isExploring);
                break;
            }
        }
    }
    
    public ZoneData GetSelectedZone()
    {
        return _selectedZone;
    }
    
    public void ShowMessage(string message)
    {
        Debug.Log($"[ZoneSelectionUI] {message}");
        if (_messageText != null)
            _messageText.text = message;
    }
    
    public void ClosePanel()
    {
        if (_explorationPanel != null)
            _explorationPanel.SetActive(false);
        _selectedZone = null;
    }
}