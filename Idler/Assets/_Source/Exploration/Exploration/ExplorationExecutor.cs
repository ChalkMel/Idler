using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class ExplorationExecutor : MonoBehaviour
{
    [Header("Services")]
    [SerializeField] private ZoneUnlockService _zoneUnlockService;
    [SerializeField] private SpiritUnlockService _spiritUnlockService;
    [SerializeField] private FrogMover _frogMover;
    [SerializeField] private ZoneSelectionUI _zoneSelectionUI;
    [SerializeField] private ExplorationTimerUI _timerUI;
    [SerializeField] private SpiritFoundPopupUI _popupUI;
    
    private ExplorationData _currentExploration;
    private bool _isExploring;
    private Transform _selectedZoneButtonTransform;
    
    public event System.Action<ZoneData> OnExplorationStarted;
    public event System.Action<ZoneData, SpiritData> OnExplorationCompleted;
    
    private void Start()
    {
        if (_zoneSelectionUI != null)
        {
            _zoneSelectionUI.OnExplorationConfirmed += StartExploration;
            _zoneSelectionUI.OnExplorationCancelled += CancelExploration;
        }
        
        if (_frogMover != null)
        {
            _frogMover.OnMovementStarted += () => _isExploring = true;
            _frogMover.OnMovementCompleted += OnMovementCompleted;
        }
    }
    
    private void Update()
    {
        if (_currentExploration != null && _currentExploration.isExploring)
        {
            _currentExploration.Update(Time.deltaTime);
            _timerUI?.UpdateTimer(_currentExploration.timeRemaining, _currentExploration.zone.explorationTime);
            
            if (_currentExploration.IsComplete)
            {
                CompleteExploration();
            }
        }
    }
    
    public void StartExploration()
    {
        if (_isExploring)
        {
            _zoneSelectionUI?.ShowMessage("Already exploring!");
            return;
        }
        
        ZoneData selectedZone = _zoneSelectionUI?.GetSelectedZone();
        if (selectedZone == null)
        {
            _zoneSelectionUI?.ShowMessage("No zone selected!");
            return;
        }
        
        if (_zoneUnlockService != null && _zoneUnlockService.IsZoneComplete(selectedZone))
        {
            _zoneSelectionUI?.ShowMessage($"All spirits in {selectedZone.zoneName} already found!");
            _zoneSelectionUI?.ClosePanel();
            return;
        }
        
        _selectedZoneButtonTransform = FindZoneButtonTransform(selectedZone);
        if (_selectedZoneButtonTransform == null)
        {
            _zoneSelectionUI?.ShowMessage($"Could not find button for zone {selectedZone.zoneName}");
            _zoneSelectionUI?.ClosePanel();
            return;
        }
        
        _zoneSelectionUI?.ClosePanel();
        
        SetAllZonesInteractable(false);
        
        _currentExploration = new ExplorationData(selectedZone);
        _timerUI?.ShowTimer(_currentExploration.timeRemaining, selectedZone.explorationTime);
        
        StartCoroutine(_frogMover.MoveToTargetAndBack(_selectedZoneButtonTransform));
        
        OnExplorationStarted?.Invoke(selectedZone);
    }
    
    private void OnMovementCompleted()
    {
        
    }
    
    private void CompleteExploration()
    {
        ZoneData exploredZone = _currentExploration.zone;
        
        SpiritData foundSpirit = null;
        if (_spiritUnlockService != null)
        {
            foundSpirit = _zoneUnlockService?.GetRandomUnfoundSpirit(exploredZone);
            if (foundSpirit != null)
            {
                _spiritUnlockService.TryUnlockSpirit(foundSpirit);
                _popupUI?.ShowSpiritFound(foundSpirit);
            }
        }
        
        string message = foundSpirit != null 
            ? $"You explored: {exploredZone.zoneName}\nFound new spirit: {foundSpirit.spiritName}"
            : $"You explored: {exploredZone.zoneName}\nAll spirits already found!";
        
        _zoneSelectionUI?.ShowMessage(message);
        
        _currentExploration = null;
        _isExploring = false;
        
        _timerUI?.HideTimer();
        
        SetAllZonesInteractable(true);

        UpdateAllZoneButtons();
        
        OnExplorationCompleted?.Invoke(exploredZone, foundSpirit);
    }
    
    public void SelectZone(ZoneData zone)
    {
        Debug.Log($"Zone selected: {zone.zoneName}");
    
        if (_isExploring)
        {
            _zoneSelectionUI?.ShowMessage("Already exploring!");
            return;
        }
    
        if (_zoneUnlockService != null && _zoneUnlockService.IsZoneComplete(zone))
        {
            _zoneSelectionUI?.ShowMessage($"All spirits in {zone.zoneName} already found!");
            return;
        }
    
        _zoneSelectionUI?.SelectZone(zone);
    }
    
    private void CancelExploration()
    {
        _currentExploration = null;
        _isExploring = false;
        _timerUI?.HideTimer();
        _zoneSelectionUI?.ClosePanel();
        SetAllZonesInteractable(true);
    }
    
    private Transform FindZoneButtonTransform(ZoneData zone)
    {
        ZoneButton[] buttons = FindObjectsByType<ZoneButton>(FindObjectsSortMode.None);
        foreach (var button in buttons)
        {
            if (button.ZoneData == zone)
                return button.transform;
        }
        return null;
    }
    
    private void SetAllZonesInteractable(bool interactable)
    {
        ZoneButton[] buttons = FindObjectsByType<ZoneButton>(FindObjectsSortMode.None);
        foreach (var button in buttons)
        {
            Button btn = button.GetComponent<Button>();
            if (btn != null)
            {
                if (interactable && button.ZoneData != null && _zoneUnlockService != null)
                {
                    bool isComplete = _zoneUnlockService.IsZoneComplete(button.ZoneData);
                    btn.interactable = button.ZoneData.isUnlocked && !isComplete;
                }
                else
                {
                    btn.interactable = interactable;
                }
            }
        }
    }
    
    private void UpdateAllZoneButtons()
    {
        if (_zoneUnlockService == null) return;
        
        ZoneButton[] buttons = FindObjectsByType<ZoneButton>(FindObjectsSortMode.None);
        foreach (var button in buttons)
        {
            if (button.ZoneData != null)
            {
                bool isComplete = _zoneUnlockService.IsZoneComplete(button.ZoneData);
                button.UpdateVisual(isComplete, button.ZoneData.isUnlocked, _isExploring);
            }
        }
    }
    
    public bool IsExploring => _isExploring;
}