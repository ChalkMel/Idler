// VisitorUI.cs
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class VisitorUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _visitorPanel;
    [SerializeField] private CanvasGroup _visitorTalking;
    [SerializeField] private GameObject _orderUI;
    
    [Header("Spirit Info")]
    [SerializeField] private Image _spiritIcon;
    [SerializeField] private TextMeshProUGUI _spiritNameText;
    [SerializeField] private TextMeshProUGUI _requestText;
    
    [Header("Requested Tea")]
    [SerializeField] private Image _requestedTeaIcon;
    [SerializeField] private TextMeshProUGUI _requestedTeaName;
    
    [Header("Buttons")]
    [SerializeField] private Button _acceptButton;
    [SerializeField] private Button _rejectButton;
    
    [Header("Timer")]
    [SerializeField] private Slider _timerSlider;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private Image _timerHandle;
    
    [Header("Sprites")]
    [SerializeField] private Sprite _onWayImage;
    [SerializeField] private Sprite _waitResponseImage;
    [SerializeField] private Sprite _waitImage;
    
    [Header("Movement")]
    [SerializeField] private RectTransform _startPoint;
    [SerializeField] private RectTransform _endPoint;
    [SerializeField] private float _moveDuration = 1.5f;
    [SerializeField] private Ease _moveEase = Ease.OutBack;
    
    public Button AcceptButton => _acceptButton;
    public Button RejectButton => _rejectButton;
    
    private Coroutine _moveCoroutine;
    private bool _isWaitingForResponse;
    private bool _isWaitingForTea;
    
    public void ShowVisitor(SpiritData spirit, string requestText)
    {
        if (_visitorPanel == null) return;
        
        _visitorPanel.SetActive(true);
        _visitorPanel.transform.position = _startPoint.position;
        
        if (_spiritIcon != null && spirit.icon != null)
            _spiritIcon.sprite = spirit.icon;
        
        if (_spiritNameText != null)
            _spiritNameText.text = spirit.spiritName;
        
        if (_requestText != null)
            _requestText.text = requestText;
        
        if (_visitorTalking != null)
            _visitorTalking.DOFade(1, _moveDuration);
        
        _moveCoroutine = StartCoroutine(MoveToCenter());
    }
    
    private IEnumerator MoveToCenter()
    {
        _visitorPanel.transform.DOMove(_endPoint.position, _moveDuration).SetEase(_moveEase);
        yield return new WaitForSeconds(_moveDuration);
    }
    
    public void UpdateRequestDisplay(OrderData order)
    {
        if (order == null) return;
        
        TeaData nextTea = order.GetNextRequiredTea();
        if (nextTea != null)
        {
            if (_requestedTeaIcon != null && nextTea.icon != null)
                _requestedTeaIcon.sprite = nextTea.icon;
            
            if (_requestedTeaName != null)
                _requestedTeaName.text = nextTea.teaName;
        }
        
        if (_requestText != null)
            _requestText.text = order.GetOrderText();
    }
    
    public void SetNextVisitTimer(float timeLeft, float maxTime)
    {
        if (_timerSlider != null)
        {
            _timerSlider.gameObject.SetActive(true);
            _timerSlider.maxValue = maxTime;
            _timerSlider.value = maxTime - timeLeft;
            
            if (_timerHandle != null && _onWayImage != null)
                _timerHandle.sprite = _onWayImage;
        }
        
        if (_timerText != null)
            _timerText.text = $"Next guest: {Mathf.CeilToInt(timeLeft)}s";
    }
    
    public void SetResponseTimer(float maxTime, float currentTime)
    {
        _isWaitingForResponse = true;
        
        if (_timerSlider != null)
        {
            _timerSlider.gameObject.SetActive(true);
            _timerSlider.maxValue = maxTime;
            _timerSlider.value = maxTime - currentTime;
            
            if (_timerHandle != null && _waitResponseImage != null)
                _timerHandle.sprite = _waitResponseImage;
        }
        
        if (_timerText != null)
            _timerText.text = $"Decide: {Mathf.CeilToInt(currentTime)}s";
    }
    
    public void SetWaitTimer(float maxTime, float currentTime)
    {
        _isWaitingForTea = true;
        
        if (_timerSlider != null)
        {
            _timerSlider.gameObject.SetActive(true);
            _timerSlider.maxValue = maxTime;
            _timerSlider.value = maxTime - currentTime;
            
            if (_timerHandle != null && _waitImage != null)
                _timerHandle.sprite = _waitImage;
        }
        
        if (_timerText != null)
            _timerText.text = $"Time left: {Mathf.CeilToInt(currentTime)}s";
    }
    
    public void ResetTimerToWaiting()
    {
        _isWaitingForResponse = false;
        _isWaitingForTea = false;
    }
    
    public void ShowOrderUI(bool show)
    {
        if (_orderUI != null)
            _orderUI.SetActive(show);
    }
    
    public void HideVisitor()
    {
        if (_visitorPanel == null) return;
        
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);
        
        _visitorPanel.transform.DOMove(_startPoint.position, _moveDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                _visitorPanel.SetActive(false);
                if (_visitorTalking != null)
                    _visitorTalking.DOFade(0, 0);
            });
        
        if (_requestedTeaIcon != null)
            _requestedTeaIcon.gameObject.SetActive(false);
    }
    
    public void ResetUI()
    {
        TextMeshProUGUI acceptText = _acceptButton?.GetComponentInChildren<TextMeshProUGUI>();
        if (acceptText != null)
            acceptText.text = "Accept";
        
        _acceptButton.interactable = true;
        _acceptButton.gameObject.SetActive(true);
        _isWaitingForResponse = false;
        _isWaitingForTea = false;
        ShowOrderUI(true);
    }

    public void HideButton()
    {
        _acceptButton.gameObject.SetActive(false);
    }
    
    public void ShowMessage(string message)
    {
        Debug.Log($"[VisitorUI] {message}");
    }
}