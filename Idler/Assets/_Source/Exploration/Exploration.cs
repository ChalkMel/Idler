using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class Exploration : MonoBehaviour
{
    [SerializeField] private ZoneData[] availableZones;
    [SerializeField] private SpiritCollection spiritCollection;
    [SerializeField] private GameObject frogSprite;
    [SerializeField] private GameObject frogMover;
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
    [SerializeField] private TextMeshProUGUI frogTalks;
    [Header("Spirit Found UI")]
    [SerializeField] private GameObject spiritFoundPanel;
    [SerializeField] private Image foundSpiritIcon;
    [SerializeField] private TextMeshProUGUI foundSpiritName;
    [SerializeField] private TextMeshProUGUI foundSpiritDescription;
    [SerializeField] private Button closeSpiritPanelButton;
    
    private ZoneData _selectedZone;
    private bool _isExploring;
    private float _explorationTimer;
    private Vector3 _originalFrogPosition;
    private Transform _selectedZoneButtonTransform;
    private string _lastExploredZoneName;

    private Dictionary<ZoneData, Button> _zoneButtons = new Dictionary<ZoneData, Button>();
    
    private void Start()
    {
        confirmButton.onClick.AddListener(StartExploration);
        cancelButton.onClick.AddListener(CancelExploration);
        closeSpiritPanelButton.onClick.AddListener(CloseSpiritFoundPanel);
        
        explorationPanel.SetActive(false);
        spiritFoundPanel.SetActive(false);
        
        _originalFrogPosition = frogMover.transform.position;
        
        InitializeZoneButtons();

        UpdateZoneButtons();
    }
    
    private void InitializeZoneButtons()
    {
        ZoneButton[] zoneButtonComponents = FindObjectsByType<ZoneButton>(FindObjectsSortMode.None);
        
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

            UpdateZoneButtonVisual(button, zone, isZoneComplete);

            button.interactable = isZoneUnlocked && !isZoneComplete && !_isExploring;
        }
    }
    
    private void UpdateZoneButtonVisual(Button button, ZoneData zone, bool isComplete)
    {
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            if (isComplete)
            {
                buttonImage.color = Color.green;
            }
            else
            {
                buttonImage.color = Color.white;
            }
        }
        
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = isComplete ? $"{zone.zoneName} ✓" : zone.zoneName;
        
    }
    
    public void SelectZone(int zoneIndex)
    {
        if (zoneIndex < 0 || zoneIndex >= availableZones.Length || _isExploring)
            return;
            
        _selectedZone = availableZones[zoneIndex];

        if (_selectedZone.AreAllSpiritsFound(spiritCollection))
        {
            ShowResultPopup($"All spirits in {_selectedZone.zoneName} already found!");
            return;
        }
        
        FindAndSetZoneButtonTransform();
        
        if (_selectedZoneButtonTransform == null)
        {
            ShowResultPopup($"Could not find button for zone {_selectedZone.zoneName}");
            return;
        }
        
        zoneNameText.text = _selectedZone.zoneName;
        zoneDescriptionText.text = _selectedZone.zoneDescription;
        zoneIconImage.sprite = _selectedZone.zoneIcon;

        string spiritsInfo = GetZoneSpiritsInfo(_selectedZone);
        timeText.text = $"Time: {_selectedZone.explorationTime} sec\n{spiritsInfo}";
        
        explorationPanel.SetActive(true);
    }

    private void FindAndSetZoneButtonTransform()
    {
        _selectedZoneButtonTransform = null;
        
        foreach (var kvp in _zoneButtons)
        {
            if (kvp.Key == _selectedZone)
            {
                if (kvp.Value != null)
                {
                    _selectedZoneButtonTransform = kvp.Value.transform;
                }
                break;
            }
        }
 
        if (_selectedZoneButtonTransform == null)
        {
            ZoneButton[] zoneButtons = FindObjectsByType<ZoneButton>(FindObjectsSortMode.None);
            foreach (var zoneButton in zoneButtons)
            {
                if (zoneButton.zoneIndex >= 0 && 
                    zoneButton.zoneIndex < availableZones.Length && 
                    availableZones[zoneButton.zoneIndex] == _selectedZone)
                {
                    _selectedZoneButtonTransform = zoneButton.transform;
                    break;
                }
            }
        }
    }
    
    private string GetZoneSpiritsInfo(ZoneData zone)
    {
        if (zone.availableSpirits.Count == 0)
            return "There is no spirits";
            
        List<SpiritData> unfoundSpirits = zone.GetUnfoundSpirits(spiritCollection);
        int foundCount = zone.availableSpirits.Count - unfoundSpirits.Count;
        
        return $"Spirits: {foundCount}/{zone.availableSpirits.Count} found";
    }
    
    private void CompleteExploration()
    {
        _isExploring = false;
        timerSlider.gameObject.SetActive(false);
        timerMainScreenText.gameObject.SetActive(false);
        timerText.text = "No exploration yet";

        _lastExploredZoneName = _selectedZone != null ? _selectedZone.zoneName : "Unknown Zone";
        
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
            ShowResultPopup($"You already explored: {_lastExploredZoneName}\nAll spirits already found!");
        }
        
        UpdateZoneButtons();
        SetAllZoneButtonsInteractable(true);
        _selectedZone = null;
        _selectedZoneButtonTransform = null;
    }
    
    private void CloseSpiritFoundPanel()
    {
        spiritFoundPanel.SetActive(false);

        ShowResultPopup($"You explored: {_lastExploredZoneName}\nFound new spirit!");
    }
    private void StartExploration()
    {
        if (_selectedZone == null)
        {
            ShowResultPopup("No zone selected!");
            return;
        }

        if (_selectedZoneButtonTransform == null)
        {
            ShowResultPopup($"Could not find zone button for {_selectedZone.zoneName}");
            FindAndSetZoneButtonTransform();
            
            if (_selectedZoneButtonTransform == null)
            {
                explorationPanel.SetActive(false);
                _selectedZone = null;
                return;
            }
        }

        frogMover.gameObject.SetActive(true);
        
        frogSprite.transform.DOPunchScale(new Vector2(0.5f, 0.5f), 0.2f);
        frogSprite.gameObject.SetActive(false);
        explorationPanel.SetActive(false);
        
        SetAllZoneButtonsInteractable(false);

        StartCoroutine(MoveFrogToZoneAndBack());
    }
    
    private IEnumerator MoveFrogToZoneAndBack()
    {
        _isExploring = true;
        
        if (_selectedZoneButtonTransform == null)
        {
            Debug.LogError("Zone button transform is null! Can't move frog.");
            CompleteExploration();
            yield break;
        }
        
        Vector3 targetPosition = _selectedZoneButtonTransform.position;
  
        float moveDuration = 0.5f;
        frogMover.transform.DOMove(targetPosition, moveDuration).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(moveDuration);
 
        _explorationTimer = _selectedZone.explorationTime;

        yield return StartCoroutine(ExplorationCoroutine());

        yield return new WaitForSeconds(0.5f);

        frogMover.transform.DOMove(_originalFrogPosition, moveDuration).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(moveDuration);

        frogMover.gameObject.SetActive(false);
        frogSprite.gameObject.SetActive(true);

        frogSprite.transform.DOPunchScale(new Vector2(0.5f, 0.5f), 0.2f);

        CompleteExploration();
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
            timerText.text = $"Time Left: {Mathf.Round(_explorationTimer)} sec";
            timerMainScreenText.gameObject.SetActive(true);
            timerMainScreenText.text = $"Exploration: {Mathf.Round(_explorationTimer)} sec";
            yield return null;
        }
    }
    
    private void ShowSpiritFoundPopup(SpiritData spirit)
    {
        foundSpiritIcon.sprite = spirit.icon;
        foundSpiritIcon.preserveAspect = true;
        foundSpiritName.text = $"Found spirit: {spirit.spiritName}";
        foundSpiritDescription.text = $"{spirit.description}\n\nBoost: {spirit.buffName}\n{spirit.buffDescription}";
        spiritFoundPanel.SetActive(true);
    }
    
    private void CancelExploration()
    {
        explorationPanel.SetActive(false);
        _selectedZone = null;
        _selectedZoneButtonTransform = null;
    }
    
    private void ShowResultPopup(string message)
    {
        frogTalks.text = message;
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
}