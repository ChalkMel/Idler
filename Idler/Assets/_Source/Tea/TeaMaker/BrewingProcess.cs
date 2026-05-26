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

    public bool IsBrewing { get; private set; }

    public void StartBrewing(TeaData tea)
    {
        if (IsBrewing) return;
        if (_brewingCoroutine != null) StopCoroutine(_brewingCoroutine);
        _brewingCoroutine = StartCoroutine(BrewingRoutine(tea));
    }

    private IEnumerator BrewingRoutine(TeaData tea)
    {
        IsBrewing = true;
        OnBrewingStarted?.Invoke(tea);

        if (_brewingPanel != null) _brewingPanel.SetActive(true);
        if (_brewingTeaNameText != null) _brewingTeaNameText.text = $"Brewing: {tea.teaName}";
        if (_brewingTeaIcon != null && tea.icon != null)
        {
            _brewingTeaIcon.sprite = tea.icon;
            _brewingTeaIcon.gameObject.SetActive(true);
        }
        if (_brewingSlider != null) _brewingSlider.value = 0f;

        float brewingTime = tea.brewingTime;
        float timer = 0f;

        while (timer < brewingTime)
        {
            timer += Time.deltaTime;
            float progress = timer / brewingTime;
            if (_brewingSlider != null) _brewingSlider.value = progress;
            if (_brewingTimeText != null)
                _brewingTimeText.text = $"Time left: {Mathf.CeilToInt(brewingTime - timer)}s";
            if (_brewingTimer != null)
            {
                _brewingTimer.gameObject.SetActive(true);
                _brewingTimer.text = $"Time left: {brewingTime - timer:F0}s";
            }
            if (_brewingTimerIcon != null && tea.icon != null)
                _brewingTimerIcon.sprite = tea.icon;
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