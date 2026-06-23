using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ExplorationExecutor : MonoBehaviour
{
    [Header("Services")]
    [SerializeField] private ZoneUnlockService zoneUnlockService;
    [SerializeField] private SpiritUnlockService spiritUnlockService;
    [SerializeField] private FrogMover frogMover;
    [SerializeField] private ZoneSelectionUI zoneSelectionUI;
    [SerializeField] private ExplorationTimerUI timerUI;
    [SerializeField] private SpiritFoundPopupUI popupUI;
    [SerializeField] private Credits credits;
    [SerializeField] private List<ZoneButton> allZones;
    [Header("Exploration Cost")]
    [SerializeField] private int baseExplorationCost = 20;
    
    private ExplorationData _currentExploration;
    private bool _isExploring;
    private Transform _selectedZoneButtonTransform;
    
    private void Start()
    {
        if (zoneSelectionUI != null)
        {
            zoneSelectionUI.OnExplorationConfirmed += StartExploration;
            zoneSelectionUI.OnExplorationCancelled += CancelExploration;
        }
        
        if (frogMover != null)
        {
            frogMover.OnMovementStarted += () => _isExploring = true;
            frogMover.OnMovementCompleted += OnMovementCompleted;
        }
    }
    
    private void Update()
    {
        if (_currentExploration != null && _currentExploration.IsExploring)
        {
            _currentExploration.Update(Time.deltaTime);
            timerUI?.UpdateTimer(_currentExploration.TimeRemaining, _currentExploration.Zone.ExplorationTime);
            
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
    return Mathf.RoundToInt(baseExplorationCost * zone.ExplorationTime / 10f);
}
    
public void StartExploration()
{
    Debug.Log("StartExploration called");
    
    if (_isExploring)
    {
        zoneSelectionUI?.ShowMessage("Уже изучаем!");
        return;
    }
    
    ZoneData selectedZone = zoneSelectionUI?.GetSelectedZone();
    if (selectedZone == null)
    {
        zoneSelectionUI?.ShowMessage("Не выбрана зона!");
        return;
    }
    
    int cost = GetExplorationCost(selectedZone);
    if (credits.droplets < cost)
    {
        zoneSelectionUI?.ShowMessage($"Недостаточно капель! Нужно {cost}");
        return;
    }
    
    if (zoneUnlockService != null && zoneUnlockService.IsZoneComplete(selectedZone))
    {
        zoneSelectionUI?.ShowMessage($"Все духи в {selectedZone.ZoneName} уже найдены!");
        zoneSelectionUI?.ClosePanel();
        return;
    }
    
    credits.droplets -= cost;
    credits.UpdateUI();
    
    _selectedZoneButtonTransform = FindZoneButtonTransform(selectedZone);
    if (_selectedZoneButtonTransform == null)
    {
        zoneSelectionUI?.ShowMessage("Не найдена кнопка зоны!");
        zoneSelectionUI?.ClosePanel();
        return;
    }
    
    zoneSelectionUI?.ClosePanel();
    
    SetAllZonesInteractable(false);
    
    _currentExploration = new ExplorationData(selectedZone);
    timerUI?.ShowTimer(_currentExploration.TimeRemaining, selectedZone.ExplorationTime);
    
    StartCoroutine(frogMover.MoveToTargetAndBack(_selectedZoneButtonTransform));
}

private void CancelExploration()
{
    Debug.Log("CancelExploration called");
    
    _currentExploration = null;
    _isExploring = false;
    timerUI?.HideTimer();
    zoneSelectionUI?.ClosePanel();
    SetAllZonesInteractable(true);
    zoneSelectionUI?.RefreshAllButtonsState();
}


private void CompleteExploration()
{
    Debug.Log("CompleteExploration called");
    
    ZoneData exploredZone = _currentExploration.Zone;
    
    SpiritData foundSpirit = null;
    if (spiritUnlockService != null)
    {
        foundSpirit = zoneUnlockService?.GetRandomUnfoundSpirit(exploredZone);
        if (foundSpirit != null)
        {
            spiritUnlockService.TryUnlockSpirit(foundSpirit);
            popupUI?.ShowSpiritFound(foundSpirit);
        }
    }
    
    string message = foundSpirit != null 
        ? $"Ты изучил: {exploredZone.ZoneName}\nНашел: {foundSpirit.SpiritName}"
        : $"Ты изучил: {exploredZone.ZoneName}\nВсе духи найдены!";
    
    zoneSelectionUI?.ShowMessage(message);
    
    _currentExploration = null;
    _isExploring = false;
    
    timerUI?.HideTimer();
    
    SetAllZonesInteractable(true);
    
    zoneSelectionUI?.RefreshAllButtonsState();
    
    
}
    
    public void SelectZone(ZoneData zone)
    {
        if (_isExploring)
        {
            zoneSelectionUI?.ShowMessage("Already exploring!");
            return;
        }
    
        if (zoneUnlockService != null && zoneUnlockService.IsZoneComplete(zone))
        {
            zoneSelectionUI?.ShowMessage($"Все духи в {zone.ZoneName} найдены!");
            return;
        }
        
        int cost = GetExplorationCost(zone);
        zoneSelectionUI?.UpdateCostDisplay(cost);
    
        zoneSelectionUI?.SelectZone(zone);
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
        foreach (var button in allZones)
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
        _currentExploration.TimeRemaining = remainingTime;
        _currentExploration.IsExploring = true;
        _isExploring = true;
        timerUI?.ShowTimer(remainingTime, zone.ExplorationTime);
    }
}