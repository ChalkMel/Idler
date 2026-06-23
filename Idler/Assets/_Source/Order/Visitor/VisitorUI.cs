using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Serialization;

public class VisitorUI : MonoBehaviour
{
    [Header("Panels")] 
    [SerializeField] private GameObject visitorPanel;
    [SerializeField] private CanvasGroup visitorTalking;
    [SerializeField] private GameObject orderUI;

    
    [Header("Spirit Info")] 
    [SerializeField] private Image spiritIcon;

    [SerializeField] private TextMeshProUGUI spiritNameText;
    [SerializeField] private TextMeshProUGUI requestText;
    
    [Header("Requested Tea")] 
    [SerializeField] private Image requestedTeaIcon;

    [SerializeField] private TextMeshProUGUI requestedTeaName;
    
    [Header("Buttons")] 
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button rejectButton;

   [Header("Timer")]
    [SerializeField] private Slider timerSlider;
    [SerializeField] private Image timerHandle;
    
    [Header("Sprites")] 
    [SerializeField] private Sprite onWayImage;
    [SerializeField] private Sprite waitResponseImage;
    [SerializeField] private Sprite waitImage;

    
    [Header("Movement")] 
    [SerializeField] private RectTransform startPoint;
    [SerializeField] private RectTransform endPoint;
    [SerializeField] private float moveDuration = 1.5f;
    [SerializeField] private Ease moveEase = Ease.OutBack;

    public Button AcceptButton => acceptButton;
    public Button RejectButton => rejectButton;

    private Coroutine _moveCoroutine;
    private bool _isWaitingForResponse;
    private bool _isWaitingForTea;

    public void ShowVisitor(SpiritData spirit, string requestText)
    {
        if (visitorPanel == null) return;

        visitorPanel.SetActive(true);
        visitorPanel.transform.position = startPoint.position;

        if (spiritIcon != null && spirit.Icon != null)
            spiritIcon.sprite = spirit.Icon;

        if (spiritNameText != null)
            spiritNameText.text = spirit.SpiritName;

        if (this.requestText != null)
            this.requestText.text = requestText;

        if (visitorTalking != null)
            visitorTalking.DOFade(1, moveDuration);

        _moveCoroutine = StartCoroutine(MoveToCenter());
    }

    private IEnumerator MoveToCenter()
    {
        visitorPanel.transform.DOMove(endPoint.position, moveDuration).SetEase(moveEase);
        yield return new WaitForSeconds(moveDuration);
    }

    public void UpdateRequestDisplay(OrderData order)
    {
        if (order == null) return;

        TeaData nextTea = order.GetNextRequiredTea();
        if (nextTea != null)
        {
            if (requestedTeaIcon != null && nextTea.Icon != null)
                requestedTeaIcon.sprite = nextTea.Icon;

            if (requestedTeaName != null)
                requestedTeaName.text = nextTea.TeaName;
        }

        if (requestText != null)
            requestText.text = order.GetOrderText();
    }

    public void SetNextVisitTimer(float timeLeft, float maxTime)
    {
        if (timerSlider != null)
        {
            timerSlider.gameObject.SetActive(true);
            timerSlider.maxValue = maxTime;
            timerSlider.value = maxTime - timeLeft;

            if (timerHandle != null && onWayImage != null)
                timerHandle.sprite = onWayImage;
        }
    }

    public void SetResponseTimer(float maxTime, float currentTime)
    {
        _isWaitingForResponse = true;

        if (timerSlider != null)
        {
            timerSlider.gameObject.SetActive(true);
            timerSlider.maxValue = maxTime;
            timerSlider.value = maxTime - currentTime;

            if (timerHandle != null && waitResponseImage != null)
                timerHandle.sprite = waitResponseImage;
        }
    }

    public void SetWaitTimer(float maxTime, float currentTime)
    {
        _isWaitingForTea = true;

        if (timerSlider != null)
        {
            timerSlider.gameObject.SetActive(true);
            timerSlider.maxValue = maxTime;
            timerSlider.value = maxTime - currentTime;

            if (timerHandle != null && waitImage != null)
                timerHandle.sprite = waitImage;
        }
    }

    public void ShowOrderUI(bool show)
    {
        if (orderUI != null)
            orderUI.SetActive(show);
    }

    public void HideVisitor()
    {
        if (visitorPanel == null) return;

        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        visitorPanel.transform.DOMove(startPoint.position, moveDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                visitorPanel.SetActive(false);
                if (visitorTalking != null)
                    visitorTalking.DOFade(0, 0);
            });

        if (requestedTeaIcon != null)
            requestedTeaIcon.gameObject.SetActive(false);
    }

    public void ResetUI()
    {
        TextMeshProUGUI acceptText = acceptButton?.GetComponentInChildren<TextMeshProUGUI>();
        if (acceptText != null)
            acceptText.text = "Accept";

        if (acceptButton != null)
        {
            acceptButton.interactable = true;
            acceptButton.gameObject.SetActive(true);
        }

        _isWaitingForResponse = false;
        _isWaitingForTea = false;
        ShowOrderUI(true);
    }

    public void HideButton()
    {
        acceptButton.gameObject.SetActive(false);
    }
}