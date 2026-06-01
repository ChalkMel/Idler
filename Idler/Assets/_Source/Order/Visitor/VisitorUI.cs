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
    
    [Header("Timers")]
    //[SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _sliderHandle;
    [SerializeField] private Slider _subSlider;
    
    [Header("Movement")]
    [SerializeField] private RectTransform _startPoint;
    [SerializeField] private RectTransform _endPoint;
    [SerializeField] private float _moveDuration = 1.5f;
    [SerializeField] private Ease _moveEase = Ease.OutBack;
    
    [Header("Sprites")]
    [SerializeField] private Sprite _onWayImage;
    [SerializeField] private Sprite _waitResponseImage;
    [SerializeField] private Sprite _waitImage;
    
    public Button AcceptButton => _acceptButton;
    public Button RejectButton => _rejectButton;
    
    private Coroutine _moveCoroutine;
    
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
    
    public void SetResponseTimer(float maxTime, float currentTime)
    {
        if (_slider != null)
        {
            _slider.maxValue = maxTime;
            _slider.value = maxTime - currentTime;
            if (_sliderHandle != null && _waitResponseImage != null)
                _sliderHandle.sprite = _waitResponseImage;
        }
        
        //if (_timerText != null)
            //_timerText.text = $"{Mathf.CeilToInt(currentTime)}s";
    }
    
    public void SetWaitTimer(float maxTime, float currentTime)
    {
        _slider.gameObject.SetActive(false);
        _subSlider.gameObject.SetActive(true);
        _subSlider.maxValue = maxTime;
        _subSlider.value = maxTime - currentTime;
    }
    
    public void SetNextVisitTimer(float timeLeft)
    {
        //if (_timerText != null)
           // _timerText.text = $"Next guest: {Mathf.CeilToInt(timeLeft)}s";
        
        if (_slider != null)
        {
            _slider.maxValue = timeLeft;
            _slider.value = _slider.maxValue - timeLeft;
            if (_sliderHandle != null && _onWayImage != null)
                _sliderHandle.sprite = _onWayImage;
        }
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
        
        _slider.gameObject.SetActive(true);
        _subSlider.gameObject.SetActive(false);
        
        if (_requestedTeaIcon != null)
            _requestedTeaIcon.gameObject.SetActive(false);
    }
    
    public void ResetUI()
    {
        _slider.gameObject.SetActive(true);
        _subSlider.gameObject.SetActive(false);
        
        TextMeshProUGUI acceptText = _acceptButton?.GetComponentInChildren<TextMeshProUGUI>();
        if (acceptText != null)
            acceptText.text = "Accept";
        
        _acceptButton.interactable = true;
    }
    
    public void ShowMessage(string message)
    {
        Debug.Log($"[VisitorUI] {message}");
        // TODO: всплывающее сообщение
    }
}