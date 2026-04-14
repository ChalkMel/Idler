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
    [SerializeField] private CanvasGroup orderUI;
    
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
    [SerializeField] private float requestTimeout = 30f;
    
    [Header("Rewards")]
    [SerializeField] private int baseReward = 50;
    [SerializeField] private float rewardMultiplier = 2f;
    
    private SpiritData _currentVisitor;
    //public TeaData RequestedTea;
    public bool IsWaitingForTea;
    private float _timeUntilNextVisit;
    private Coroutine _visitorCoroutine;
    private Coroutine _timeoutCoroutine;
    [SerializeField] private int minTeasInOrder = 1;
    [SerializeField] private int maxTeasInOrder = 3;

    public List<TeaData> RequestedTeas;
    private int _completedTeasCount;
    private List<bool> _completedTeas;
    
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
        
        if (IsWaitingForTea && RequestedTeas != null)
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
        
        int teasCount = Random.Range(minTeasInOrder, maxTeasInOrder + 1);
        RequestedTeas = new List<TeaData>();
        for (int i = 0; i < teasCount; i++)
        {
            TeaData tea = teaMaker.allTeas[Random.Range(0, teaMaker.allTeas.Count)];
            RequestedTeas.Add(tea);
        }
        
        _completedTeas = new List<bool>();
        for (int i = 0; i < teasCount; i++)
        {
            _completedTeas.Add(false);
        }
        _completedTeasCount = 0;
    
        if (_currentVisitor == null || RequestedTeas == null || RequestedTeas.Count == 0) return;
    
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
            acceptButton.interactable = false;
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
        //SetSlider(requestTimeout, waitImage);
        slider.gameObject.SetActive(false);
        subSlider.gameObject.SetActive(true);
        subSlider.maxValue = timeout;
        while (timeout > 0 && IsWaitingForTea)
        {
            timeout -= Time.deltaTime;
            //UpdateSlider(timeout);
            subSlider.value = subSlider.maxValue - timeout;
            
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
        if (_currentVisitor == null || RequestedTeas == null || RequestedTeas.Count == 0) return;
    
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
            string orderText = $"Приготовь для меня:\n";
            for (int i = 0; i < RequestedTeas.Count; i++)
            {
                orderText += $"- {RequestedTeas[i].teaName}\n";
            }
            requestText.text = orderText;
        }
        
        if (requestedTeaIcon != null && RequestedTeas[0].icon != null)
        {
            requestedTeaIcon.sprite = RequestedTeas[0].icon;
            requestedTeaIcon.gameObject.SetActive(true);
        }
        
        if (requestedTeaName != null)
        {
            requestedTeaName.text = RequestedTeas[0].teaName;
            requestedTeaName.gameObject.SetActive(true);
        }

        TextMeshProUGUI acceptButtonText = acceptButton.GetComponentInChildren<TextMeshProUGUI>();
        if (acceptButtonText != null)
        {
            acceptButtonText.text = "Принять (15с)";
        }
    }
    
    private void CheckIfTeaWasBrewed()
    {
        if (teaMaker == null || RequestedTeas == null || RequestedTeas.Count == 0) return;
    
        TeaData lastBrewed = teaMaker.GetLastBrewedTea();
        teaMaker.ResetLastBrewedTea();
    
        if (lastBrewed == null) return;
        
        for (int i = 0; i < RequestedTeas.Count; i++)
        {
            if (RequestedTeas[i] == lastBrewed && !_completedTeas[i])
            {
                _completedTeas[i] = true;
                _completedTeasCount++;
            
                ShowMessage($"Чай {lastBrewed.teaName} принят! Осталось: {RequestedTeas.Count - _completedTeasCount}");
                
                UpdateCurrentRequestDisplay();
                
                if (_completedTeasCount >= RequestedTeas.Count)
                {
                    CompleteOrder();
                }
                break;
            }
        }
    }
    
    private void UpdateCurrentRequestDisplay()
    {
        if (_completedTeasCount < RequestedTeas.Count)
        {
            for (int i = 0; i < RequestedTeas.Count; i++)
            {
                if (!_completedTeas[i])
                {
                    if (requestedTeaIcon != null && RequestedTeas[i].icon != null)
                    {
                        requestedTeaIcon.sprite = RequestedTeas[i].icon;
                    }
                    break;
                }
            }
            if (requestText != null)
            {
                string orderText = $"Приготовь для меня:\n";
                for (int j = 0; j < RequestedTeas.Count; j++)
                {
                    if (!_completedTeas[j])
                        orderText += $"- {RequestedTeas[j].teaName}\n";
                }
                requestText.text = orderText;
            }
        }
    }

    public string SetOrderText()
    {
        string orderText = $"Заказ:\n";
        for (int j = 0; j < RequestedTeas.Count; j++)
        {
            if (!_completedTeas[j])
                orderText += $"- {RequestedTeas[j].teaName}\n";
        }
        return orderText;
    }
    
    private void CompleteOrder()
    {
        int reward = Mathf.RoundToInt(baseReward * rewardMultiplier * RequestedTeas.Count);
        credits.droplets += reward * _completedTeasCount;
    
        ShowMessage($"Дух доволен! +{reward} капель");
        orderUI.gameObject.SetActive(true);
        EndVisit();
    }

    private void RejectRequest()
    {
        ShowMessage("Дух ушел...");
        EndVisit();
    }
    
    private void EndVisit()
    {
        acceptButton.interactable = true;
        IsWaitingForTea = false;
        subSlider.gameObject.SetActive(false);
        slider.gameObject.SetActive(true);
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
                ScheduleNextVisit();
                requestedTeaIcon.gameObject.SetActive(false);
                //requestedTeaName.gameObject.SetActive(false);
                visitorPanel.SetActive(false);
                
                RequestedTeas = null;
                _completedTeas = null;
                _completedTeasCount = 0;
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