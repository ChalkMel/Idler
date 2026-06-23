using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class BrewingProcess : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject _brewingPanel;
    [SerializeField] private UnityEngine.UI.Slider _brewingSlider;
    [SerializeField] private TMPro.TextMeshProUGUI _brewingTimeText;
    [SerializeField] private TMPro.TextMeshProUGUI _brewingTeaNameText;
    [SerializeField] private UnityEngine.UI.Image _brewingTeaIcon;
    [SerializeField] private TMPro.TextMeshProUGUI _brewingTimer;
    [SerializeField] private UnityEngine.UI.Image _brewingTimerIcon;

    public UnityEvent<TeaData> OnBrewingStarted;
    public UnityEvent<TeaData> OnBrewingCompleted;

    private Coroutine _brewingCoroutine;
    private TeaData _currentTea;
    private float _elapsedTime;
    private float _totalTime;

    public bool IsBrewing { get; private set; }
    public TeaData GetCurrentTea() => _currentTea;
    public float GetElapsedTime() => _elapsedTime;

    public void StartBrewing(TeaData tea)
    {
        if (IsBrewing) return;
        if (_brewingCoroutine != null) StopCoroutine(_brewingCoroutine);
        _brewingCoroutine = StartCoroutine(BrewingRoutine(tea));
    }
    
    public void LoadBrewing(TeaData tea, float remainingTime)
    {
        if (IsBrewing) return;
        if (_brewingCoroutine != null) StopCoroutine(_brewingCoroutine);
        _brewingCoroutine = StartCoroutine(BrewingRoutine(tea, remainingTime));
    }

    private IEnumerator BrewingRoutine(TeaData tea, float startRemainingTime = -1)
    {
        IsBrewing = true;
        _currentTea = tea;
        _totalTime = tea.BrewingTime;
        
        if (startRemainingTime > 0)
            _elapsedTime = _totalTime - startRemainingTime;
        else
            _elapsedTime = 0;
        
        OnBrewingStarted?.Invoke(tea);

        if (_brewingPanel != null) _brewingPanel.SetActive(true);
        if (_brewingTeaNameText != null) _brewingTeaNameText.text = $"Варим: {tea.TeaName}";
        if (_brewingTeaIcon != null && tea.Icon != null)
        {
            _brewingTeaIcon.sprite = tea.Icon;
            _brewingTeaIcon.gameObject.SetActive(true);
        }
        if (_brewingSlider != null) _brewingSlider.value = _elapsedTime / _totalTime;

        while (_elapsedTime < _totalTime)
        {
            _elapsedTime += Time.deltaTime;
            float progress = _elapsedTime / _totalTime;
            
            if (_brewingSlider != null) _brewingSlider.value = progress;
            if (_brewingTimeText != null)
                _brewingTimeText.text = $"Осталось: {Mathf.CeilToInt(_totalTime - _elapsedTime)}s";
            if (_brewingTimer != null)
            {
                _brewingTimer.gameObject.SetActive(true);
                _brewingTimer.text = $"{_totalTime - _elapsedTime:F0}s";
            }
            if (_brewingTimerIcon != null && tea.Icon != null)
                _brewingTimerIcon.sprite = tea.Icon;
            yield return null;
        }
        
        if (_brewingPanel != null) _brewingPanel.SetActive(false);
        if (_brewingTeaIcon != null) _brewingTeaIcon.gameObject.SetActive(false);
        if (_brewingTimer != null) _brewingTimer.gameObject.SetActive(false);
        if (_brewingTimerIcon != null) _brewingTimerIcon.gameObject.SetActive(false);

        IsBrewing = false;
        OnBrewingCompleted?.Invoke(tea);
        
        _brewingCoroutine = null;
    }

    public void CancelBrewing()
    {
        if (_brewingCoroutine != null)
        {
            StopCoroutine(_brewingCoroutine);
            _brewingCoroutine = null;
        }
        IsBrewing = false;
        if (_brewingPanel != null) _brewingPanel.SetActive(false);
    }
}