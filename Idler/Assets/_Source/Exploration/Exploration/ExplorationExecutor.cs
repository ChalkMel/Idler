// ExplorationExecutor.cs
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Credits _credits;
    [SerializeField] private List<ZoneButton> _allZones;
    [Header("Exploration Cost")]
    [SerializeField] private int _baseExplorationCost = 20;
    
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
    
    private void OnMovementCompleted()
    {
    }
    
    public int GetExplorationCost(ZoneData zone)
{
    return Mathf.RoundToInt(_baseExplorationCost * zone.explorationTime / 10f);
}

// И исправить StartExploration - добавить сброс состояния
public void StartExploration()
{
    Debug.Log("StartExploration called");
    
    if (_isExploring)
    {
        _zoneSelectionUI?.ShowMessage("Уже изучаем!");
        return;
    }
    
    ZoneData selectedZone = _zoneSelectionUI?.GetSelectedZone();
    if (selectedZone == null)
    {
        _zoneSelectionUI?.ShowMessage("Не выбрана зона!");
        return;
    }
    
    int cost = GetExplorationCost(selectedZone);
    if (_credits.droplets < cost)
    {
        _zoneSelectionUI?.ShowMessage($"Недостаточно капель! Нужно {cost}");
        return;
    }
    
    if (_zoneUnlockService != null && _zoneUnlockService.IsZoneComplete(selectedZone))
    {
        _zoneSelectionUI?.ShowMessage($"Все духи в {selectedZone.zoneName} уже найдены!");
        _zoneSelectionUI?.ClosePanel();
        return;
    }
    
    _credits.droplets -= cost;
    _credits.UpdateUI();
    
    _selectedZoneButtonTransform = FindZoneButtonTransform(selectedZone);
    if (_selectedZoneButtonTransform == null)
    {
        _zoneSelectionUI?.ShowMessage("Не найдена кнопка зоны!");
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

// Исправить CancelExploration
private void CancelExploration()
{
    Debug.Log("CancelExploration called");
    
    _currentExploration = null;
    _isExploring = false;
    _timerUI?.HideTimer();
    _zoneSelectionUI?.ClosePanel();
    SetAllZonesInteractable(true);
    _zoneSelectionUI?.RefreshAllButtonsState();
}

// Исправить CompleteExploration
private void CompleteExploration()
{
    Debug.Log("CompleteExploration called");
    
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
        ? $"Ты изучил: {exploredZone.zoneName}\nНашел: {foundSpirit.spiritName}"
        : $"Ты изучил: {exploredZone.zoneName}\nВсе духи найдены!";
    
    _zoneSelectionUI?.ShowMessage(message);
    
    _currentExploration = null;
    _isExploring = false;
    
    _timerUI?.HideTimer();
    
    SetAllZonesInteractable(true);
    
    _zoneSelectionUI?.RefreshAllButtonsState();
    
    OnExplorationCompleted?.Invoke(exploredZone, foundSpirit);
}
    
    public void SelectZone(ZoneData zone)
    {
        if (_isExploring)
        {
            _zoneSelectionUI?.ShowMessage("Already exploring!");
            return;
        }
    
        if (_zoneUnlockService != null && _zoneUnlockService.IsZoneComplete(zone))
        {
            _zoneSelectionUI?.ShowMessage($"Все духи в {zone.zoneName} найдены!");
            return;
        }
        
        int cost = GetExplorationCost(zone);
        _zoneSelectionUI?.UpdateCostDisplay(cost);
    
        _zoneSelectionUI?.SelectZone(zone);
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
        foreach (var button in _allZones)
        {
            Button btn = button.GetComponent<Button>();
            if (btn != null)
            {
                btn.interactable = interactable;
            }
        }
    }
    
    public bool IsExploring => _isExploring;
    public ExplorationData GetCurrentExploration() => _currentExploration;
    
    public void LoadExploration(ZoneData zone, float remainingTime)
    {
        _currentExploration = new ExplorationData(zone);
        _currentExploration.timeRemaining = remainingTime;
        _currentExploration.isExploring = true;
        _isExploring = true;
        _timerUI?.ShowTimer(remainingTime, zone.explorationTime);
    }
}