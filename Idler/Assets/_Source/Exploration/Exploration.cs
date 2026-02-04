using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Exploration : MonoBehaviour
{
    [SerializeField] private ZoneData[] availableZones;
    [SerializeField] private SpiritCollection spiritCollection;
    
    [SerializeField] private GameObject explorationPanel;
    [SerializeField] private TextMeshProUGUI zoneNameText;
    [SerializeField] private TextMeshProUGUI zoneDescriptionText;
    [SerializeField] private Image zoneIconImage;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI timerMainScreenText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Slider timerSlider;
    
    [Header("Spirit Found UI")]
    [SerializeField] private GameObject spiritFoundPanel;
    [SerializeField] private Image foundSpiritIcon;
    [SerializeField] private TextMeshProUGUI foundSpiritName;
    [SerializeField] private TextMeshProUGUI foundSpiritDescription;
    [SerializeField] private Button closeSpiritPanelButton;
    
    private ZoneData _selectedZone;
    private bool _isExploring;
    private float _explorationTimer;

    private Dictionary<ZoneData, Button> _zoneButtons = new Dictionary<ZoneData, Button>();
    
    private void Start()
    {
        confirmButton.onClick.AddListener(StartExploration);
        cancelButton.onClick.AddListener(CancelExploration);
        closeSpiritPanelButton.onClick.AddListener(CloseSpiritFoundPanel);
        
        explorationPanel.SetActive(false);
        spiritFoundPanel.SetActive(false);
        
        InitializeZoneButtons();

        UpdateZoneButtons();
    }
    
    private void InitializeZoneButtons()
    {
        ZoneButton[] zoneButtonComponents = FindObjectsOfType<ZoneButton>();
        
        foreach (var zoneButton in zoneButtonComponents)
        {
            if (zoneButton.zoneIndex >= 0 && zoneButton.zoneIndex < availableZones.Length)
            {
                ZoneData zone = availableZones[zoneButton.zoneIndex];
                Button button = zoneButton.GetComponent<Button>();
                
                if (button != null)
                {
                    _zoneButtons[zone] = button;
                }
            }
        }
    }
    
    private void UpdateZoneButtons()
    {
        foreach (var kvp in _zoneButtons)
        {
            ZoneData zone = kvp.Key;
            Button button = kvp.Value;
            
            if (!zone || !button) continue;
            
            bool isZoneComplete = zone.AreAllSpiritsFound(spiritCollection);
            bool isZoneUnlocked = zone.isUnlocked;

            UpdateZoneButtonVisual(button, zone, isZoneComplete, isZoneUnlocked);

            button.interactable = isZoneUnlocked && !isZoneComplete && !_isExploring;
        }
    }
    
    private void UpdateZoneButtonVisual(Button button, ZoneData zone, bool isComplete, bool isUnlocked)
    {
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            if (!isUnlocked)
            {
                buttonImage.color = Color.gray;
            }
            else if (isComplete)
            {
                buttonImage.color = Color.green;
            }
            else
            {
                buttonImage.color = Color.white;
            }
        }
        
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = isComplete ? $"{zone.zoneName} ✓" : zone.zoneName;
        }
    }
    
    public void SelectZone(int zoneIndex)
    {
        if (zoneIndex < 0 || zoneIndex >= availableZones.Length || _isExploring)
            return;
            
        _selectedZone = availableZones[zoneIndex];

        if (!_selectedZone.isUnlocked)
        {
            ShowResultPopup($"Зона {_selectedZone.zoneName} заблокирована!");
            return;
        }

        if (_selectedZone.AreAllSpiritsFound(spiritCollection))
        {
            ShowResultPopup($"Все духи в зоне {_selectedZone.zoneName} уже найдены!");
            return;
        }
        
        zoneNameText.text = _selectedZone.zoneName;
        zoneDescriptionText.text = _selectedZone.zoneDescription;
        zoneIconImage.sprite = _selectedZone.zoneIcon;

        string spiritsInfo = GetZoneSpiritsInfo(_selectedZone);
        timeText.text = $"Время: {_selectedZone.explorationTime} сек\n{spiritsInfo}";
        
        explorationPanel.SetActive(true);
    }
    
    private string GetZoneSpiritsInfo(ZoneData zone)
    {
        if (zone.availableSpirits.Count == 0)
            return "В зоне нет духов";
            
        List<SpiritData> unfoundSpirits = zone.GetUnfoundSpirits(spiritCollection);
        int foundCount = zone.availableSpirits.Count - unfoundSpirits.Count;
        
        return $"Духи: {foundCount}/{zone.availableSpirits.Count} найдено";
    }
    
    private void StartExploration()
    {
        if (_selectedZone == null)
            return;
            
        explorationPanel.SetActive(false);
        _isExploring = true;
        _explorationTimer = _selectedZone.explorationTime;

        SetAllZoneButtonsInteractable(false);
        
        StartCoroutine(ExplorationCoroutine());
    }
    
    private IEnumerator ExplorationCoroutine()
    {
        timerSlider.gameObject.SetActive(true);
        timerSlider.maxValue = _selectedZone.explorationTime;
        timerSlider.minValue = 0;
        
        while (_explorationTimer > 0)
        {
            _explorationTimer -= Time.deltaTime;
            timerSlider.value = _selectedZone.explorationTime - _explorationTimer;
            timerText.text = $"Времени осталось: {Mathf.Round(_explorationTimer)} сек";
            timerMainScreenText.gameObject.SetActive(true);
            timerMainScreenText.text = $"Исследование: {Mathf.Round(_explorationTimer)} сек";
            yield return null;
        }
        
        CompleteExploration();
    }
    
    private void CompleteExploration()
    {
        _isExploring = false;
        timerSlider.gameObject.SetActive(false);
        timerMainScreenText.gameObject.SetActive(false);
        timerText.text = "Пока не идет изучение";

        SpiritData foundSpirit = _selectedZone.GetRandomUnfoundSpirit(spiritCollection);
        
        if (foundSpirit != null)
        {
            if (spiritCollection.UnlockSpirit(foundSpirit))
            {
                ShowSpiritFoundPopup(foundSpirit);
            }
        }
        else
        {
            ShowResultPopup($"Вы исследовали зону: {_selectedZone.zoneName}\nВсе духи в этой зоне уже найдены!");
        }
        
        UpdateZoneButtons();
        
        SetAllZoneButtonsInteractable(true);
    }
    
    private void ShowSpiritFoundPopup(SpiritData spirit)
    {
        if (spiritFoundPanel == null) return;
        
        if (foundSpiritIcon != null && spirit.icon != null)
        {
            foundSpiritIcon.sprite = spirit.icon;
            foundSpiritIcon.preserveAspect = true;
        }
        
        if (foundSpiritName != null)
        {
            foundSpiritName.text = $"Найден дух: {spirit.spiritName}";
        }
        
        if (foundSpiritDescription != null)
        {
            foundSpiritDescription.text = $"{spirit.description}\n\nБуст: {spirit.buffName}\n{spirit.buffDescription}";
        }

        spiritFoundPanel.SetActive(true);
    }
    
    private void CloseSpiritFoundPanel()
    {
        spiritFoundPanel.SetActive(false);

        ShowResultPopup($"Вы исследовали зону: {_selectedZone.zoneName}\nНайден новый дух!");
    }
    
    private void CancelExploration()
    {
        explorationPanel.SetActive(false);
        _selectedZone = null;
    }
    
    private void ShowResultPopup(string message)
    {
        Debug.Log(message);
    }
    
    private void SetAllZoneButtonsInteractable(bool interactable)
    {
        foreach (var button in _zoneButtons.Values)
        {
            if (button != null)
            {
                button.interactable = interactable;
            }
        }
    }

    public void UnlockZone(ZoneData zone)
    {
        if (zone != null)
        {
            zone.isUnlocked = true;
            UpdateZoneButtons();
        }
    }

    public void RefreshZoneButtons()
    {
        UpdateZoneButtons();
    }
}