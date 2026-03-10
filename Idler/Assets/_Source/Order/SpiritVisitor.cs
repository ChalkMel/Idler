using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
public class SpiritVisitor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpiritCollection spiritCollection;
    [SerializeField] private Credits credits;
    [SerializeField] private TeaMaker teaMaker;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject visitorPanel;
    [SerializeField] private CanvasGroup visitorTalking;
    [SerializeField] private Image spiritIcon;
    [SerializeField] private TextMeshProUGUI spiritNameText;
    [SerializeField] private TextMeshProUGUI requestText;
    [SerializeField] private Image requestedTeaIcon;
    [SerializeField] private TextMeshProUGUI requestedTeaName;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button rejectButton;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image circleImage;
    
    [Header("Slider Settings")]
    [SerializeField] private Slider slider;
    [SerializeField] private Image sliderHandle;
    [SerializeField] private Sprite onWayImage;
    [SerializeField] private Sprite waitResponseImage;
    [SerializeField] private Sprite waitImage;
    [SerializeField] private Slider subSlider;
    
    [Header("Movement Settings")]
    [SerializeField] private RectTransform startPoint;
    [SerializeField] private RectTransform endPoint;
    [SerializeField] private float moveDuration = 1.5f;
    [SerializeField] private Ease moveEase = Ease.OutBack;
    
    [Header("Timing")]
    [SerializeField] private float minTimeBetweenVisits = 30f;
    [SerializeField] private float maxTimeBetweenVisits = 60f;
    [SerializeField] private float requestTimeout = 30f; // Сколько ждать приготовления
    
    [Header("Rewards")]
    [SerializeField] private int baseReward = 50;
    [SerializeField] private float rewardMultiplier = 2f;
    
    private SpiritData _currentVisitor;
    public List<TeaMaker> RequestedTeas;
    public TeaData RequestedTea;
    public bool IsWaitingForTea;
    private float _timeUntilNextVisit;
    private Coroutine _visitorCoroutine;
    private Coroutine _timeoutCoroutine;
    
    private void Start()
    {
        circleImage.gameObject.SetActive(false);
        visitorPanel.SetActive(false);
        
        rejectButton.onClick.AddListener(RejectRequest);
        
        ScheduleNextVisit();
    }
    
    private void Update()
    {
        if (!IsWaitingForTea && _timeUntilNextVisit > 0)
        {
            _timeUntilNextVisit -= Time.deltaTime;
            int seconds = Mathf.CeilToInt(_timeUntilNextVisit);
            timerText.text = $"Следующий гость: {seconds}с";
            UpdateSlider(_timeUntilNextVisit);
            if (_timeUntilNextVisit <= 0)
            {
                StartVisitor();
            }
        }
        
        if (IsWaitingForTea && RequestedTea != null)
        {
            CheckIfTeaWasBrewed();
        }
    }
    
    private void ScheduleNextVisit()
    {
        _timeUntilNextVisit = Random.Range(minTimeBetweenVisits, maxTimeBetweenVisits);
        SetSlider(_timeUntilNextVisit, onWayImage);
    }
    
    private void StartVisitor()
    {
        circleImage.gameObject.SetActive(true);
        
        _currentVisitor = spiritCollection.allSpirits[Random.Range(0, spiritCollection.allSpirits.Count)];
        RequestedTea = teaMaker.allTeas[Random.Range(0, teaMaker.allTeas.Count)];
        
        if (_currentVisitor == null || RequestedTea == null) return;
        if (_visitorCoroutine != null)
        {
            StopCoroutine(_visitorCoroutine);
            _visitorCoroutine = null;
        }
        
        _visitorCoroutine = StartCoroutine(VisitorRoutine());
    }
    
    private IEnumerator VisitorRoutine()
    {
        visitorPanel.transform.position = startPoint.position;
        visitorPanel.SetActive(true);
        visitorPanel.transform.DOMove(endPoint.position, moveDuration)
            .SetEase(moveEase);
        visitorTalking.DOFade(1, moveDuration);
        SetupVisitorUI();
        
        bool playerResponded = false;
        float responseTimer = 15f; 
        SetSlider(responseTimer, waitResponseImage);
        acceptButton.onClick.AddListener(() => { 
            playerResponded = true; 
            IsWaitingForTea = true;
            ShowMessage("Приготовь заказанный чай!");

            if (_timeoutCoroutine != null)
                StopCoroutine(_timeoutCoroutine);
            _timeoutCoroutine = StartCoroutine(RequestTimeoutRoutine());
        });
        
        rejectButton.onClick.AddListener(() => { 
            playerResponded = true; 
            RejectRequest();
        });
        
        while (!playerResponded && responseTimer > 0)
        {
            responseTimer -= Time.deltaTime;
            UpdateSlider(responseTimer);
            if (acceptButton != null)
            {
                TextMeshProUGUI buttonText = acceptButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = $"Accept ({Mathf.CeilToInt(responseTimer)}с)";
                }
            }
            
            yield return null;
        }
        
        if (!playerResponded)
        {
            RejectRequest();
        }
    }
    
    private IEnumerator RequestTimeoutRoutine()
    {
        float timeout = requestTimeout;
        SetSlider(requestTimeout, waitImage);
        subSlider.gameObject.SetActive(true);
        subSlider.maxValue = timeout;
        while (timeout > 0 && IsWaitingForTea)
        {
            timeout -= Time.deltaTime;
            UpdateSlider(timeout);
            subSlider.value = subSlider.maxValue - timeout;
            if (requestText != null)
            {
                requestText.text = $"Жду чай: {Mathf.CeilToInt(timeout)}с";
            }
            
            yield return null;
        }

        if (IsWaitingForTea)
        {
            ShowMessage("Дух устал ждать...");
            RejectRequest();
        }
    }
    
    private void SetupVisitorUI()
    {
        if (_currentVisitor == null || RequestedTea == null) return;
        
        
        if (spiritIcon != null && _currentVisitor.icon != null)
        {
            spiritIcon.sprite = _currentVisitor.icon;
        }
        
        if (spiritNameText != null)
        {
            spiritNameText.text = _currentVisitor.spiritName;
        }
        
        if (requestText != null)
        {
            requestText.text = $"Приготовь для меня:\n{RequestedTea.teaName}";
        }
        
        if (requestedTeaIcon != null && RequestedTea.icon != null)
        {
            requestedTeaIcon.sprite = RequestedTea.icon;
            requestedTeaIcon.gameObject.SetActive(true);
        }

        TextMeshProUGUI acceptButtonText = acceptButton.GetComponentInChildren<TextMeshProUGUI>();
        if (acceptButtonText != null)
        {
            acceptButtonText.text = "Принять (15с)";
        }
    }
    
    private void CheckIfTeaWasBrewed()
    {
        if (teaMaker == null || RequestedTea == null) return;
        
        TeaData lastBrewed = teaMaker.GetLastBrewedTea();
        
        if (lastBrewed == RequestedTea)
        {
            CompleteOrder();
        }
    }
    
    private void CompleteOrder()
    {
        int reward = Mathf.RoundToInt(baseReward * rewardMultiplier);
        credits.droplets += reward;
        
        if (!_currentVisitor.isUnlocked && spiritCollection != null)
        {
            spiritCollection.UnlockSpirit(_currentVisitor);
        }
        
        ShowMessage($"Дух доволен! +{reward} капель");
        
        EndVisit(true);
    }
    
    public void RejectRequest()
    {
        ShowMessage("Дух ушел...");
        EndVisit(false);
    }
    
    private void EndVisit(bool success)
    {
        IsWaitingForTea = false;
        subSlider.gameObject.SetActive(false);
        visitorTalking.DOFade(0, 1);
        circleImage.gameObject.SetActive(false);
        
        if (_timeoutCoroutine != null)
        {
            StopCoroutine(_timeoutCoroutine);
            _timeoutCoroutine = null;
        }
        
        visitorPanel.transform.DOMove(startPoint.position, moveDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => {
                visitorPanel.SetActive(false);
                ScheduleNextVisit();
                requestedTeaIcon.gameObject.SetActive(false);
                requestedTeaName.gameObject.SetActive(false);
                });
        
        TextMeshProUGUI acceptButtonText = acceptButton.GetComponentInChildren<TextMeshProUGUI>();
        if (acceptButtonText != null)
        {
            acceptButtonText.text = "Принять";
        }
    }
    
    private void ShowMessage(string message)
    {
        Debug.Log(message);
        //TODO всплывающее сообщение
    }

    private void SetSlider(float value, Sprite sprite)
    {
        slider.maxValue = value;
        slider.value = 0;
        sliderHandle.sprite = sprite;
    }

    private void UpdateSlider(float value)
    {
        slider.value = slider.maxValue - value;
    }
    public void ForceVisit()
    {
        if (!IsWaitingForTea)
        {
            _timeUntilNextVisit = 0;
        }
    }
}