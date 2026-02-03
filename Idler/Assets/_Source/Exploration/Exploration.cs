using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Exploration : MonoBehaviour
{
    [SerializeField] private ZoneData[] availableZones;
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
    
    private ZoneData _selectedZone;
    private bool _isExploring;
    private float _explorationTimer;
    
    private void Start()
    {
        confirmButton.onClick.AddListener(StartExploration);
        cancelButton.onClick.AddListener(CancelExploration);
        explorationPanel.SetActive(false);
    }
    
    public void SelectZone(int zoneIndex)
    {
        if (zoneIndex < 0 || zoneIndex >= availableZones.Length || _isExploring)
            return;
            
        _selectedZone = availableZones[zoneIndex];
        
        zoneNameText.text = _selectedZone.zoneName;
        zoneDescriptionText.text = _selectedZone.zoneDescription;
        zoneIconImage.sprite = _selectedZone.zoneIcon;
        timeText.text = $"Время: {_selectedZone.explorationTime} сек";
        
        
        explorationPanel.SetActive(true);
    }
    
    private void StartExploration()
    {
        if (_selectedZone == null)
            return;
            
        explorationPanel.SetActive(false);
        _isExploring = true;
        _explorationTimer = _selectedZone.explorationTime;
        
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
            timerMainScreenText.text = $"Времени осталось: {Mathf.Round(_explorationTimer)} сек";
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
        ShowResultPopup($"Вы исследовали зону: {_selectedZone.zoneName}");
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
}