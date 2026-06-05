using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VisitorSpawner : MonoBehaviour
{
    [SerializeField] private OrderGenerator _orderGenerator;
    [SerializeField] private OrderMatcher _orderMatcher;
    [SerializeField] private RewardDistributor _rewardDistributor;
    [SerializeField] private VisitorUI _visitorUI;
    [SerializeField] private TeaBrewingController _teaMaker;
    
    [Header("Timing")]
    [SerializeField] private float _minTimeBetweenVisits = 30f;
    [SerializeField] private float _maxTimeBetweenVisits = 60f;
    [SerializeField] private float _responseTimeout = 15f;
    [SerializeField] private float _requestTimeout = 30f;
    [SerializeField] private Image hint;
    
    private bool _isWaitingForResponse;
    private bool _isWaitingForTea;
    private float _timeUntilNextVisit;
    private Coroutine _responseCoroutine;
    private Coroutine _requestCoroutine;
    private AudioSource _audioSource;
    
    public bool IsWaitingForTea => _isWaitingForTea;
    
    private float _currentResponseTimeout;
    private float _currentRequestTimeout;
    
    private float _baseResponseTimeout;
    private float _baseRequestTimeout;
    
    private void Start()
    {
        _baseResponseTimeout = _responseTimeout;
        _baseRequestTimeout = _requestTimeout;
        
        ResetTimeouts();
        
        _audioSource = GetComponent<AudioSource>();
        ScheduleNextVisit();
        
        if (_visitorUI != null)
        {
            _visitorUI.AcceptButton.onClick.AddListener(AcceptOrder);
            _visitorUI.RejectButton.onClick.AddListener(RejectOrder);
        }
        
        if (_orderMatcher != null)
        {
            _orderMatcher.OnOrderCompleted += OnOrderCompleted;
        }
    }
    
    private void ResetTimeouts()
    {
        _currentResponseTimeout = _baseResponseTimeout;
        _currentRequestTimeout = _baseRequestTimeout;
    }
    
    public void ExtendOrderTime(bool affectResponse, bool affectRequest, float multiplier, float addSeconds)
    {
        if (affectResponse)
        {
            _currentResponseTimeout = (_baseResponseTimeout + addSeconds) * multiplier;
            _currentResponseTimeout = Mathf.Max(1f, _currentResponseTimeout); // минимум 1 секунда
        }
        
        if (affectRequest)
        {
            _currentRequestTimeout = (_baseRequestTimeout + addSeconds) * multiplier;
            _currentRequestTimeout = Mathf.Max(1f, _currentRequestTimeout);
        }
        
        StartRequestTimer();
    }
    
    private void Update()
    {
        if (!_isWaitingForResponse && !_isWaitingForTea && _timeUntilNextVisit > 0)
        {
            _timeUntilNextVisit -= Time.deltaTime;
            _visitorUI?.SetNextVisitTimer(_timeUntilNextVisit, _maxTimeBetweenVisits);
        
            if (_timeUntilNextVisit <= 0)
            {
                SpawnVisitor();
            }
        }
    
        if (_isWaitingForTea && _teaMaker != null)
        {
            CheckBrewedTea();
        }
    }
    
    private void ScheduleNextVisit()
    {
        _timeUntilNextVisit = Random.Range(_minTimeBetweenVisits, _maxTimeBetweenVisits);
    }
    
    private void SpawnVisitor()
    {
        hint.gameObject.SetActive(true);
        _audioSource.Play();
        OrderData order = _orderGenerator.GenerateOrder();
        if (order == null) return;
        
        _orderMatcher.SetOrder(order);
        _visitorUI.ShowVisitor(order.visitor, order.GetOrderText());
        
        _isWaitingForResponse = true;
        StartResponseTimer();
    }
    
    private void StartResponseTimer()
    {
        if (_responseCoroutine != null)
            StopCoroutine(_responseCoroutine);
        _responseCoroutine = StartCoroutine(ResponseTimerRoutine());
    }
    
    private IEnumerator ResponseTimerRoutine()
    {
        float timer = _currentResponseTimeout; // Используем динамическое значение
        
        while (timer > 0 && _isWaitingForResponse)
        {
            timer -= Time.deltaTime;
            _visitorUI?.SetResponseTimer(_currentResponseTimeout, timer);
            yield return null;
        }
        
        if (_isWaitingForResponse)
        {
            RejectOrder();
        }
    }
    
    private void StartRequestTimer()
    {
        if (_requestCoroutine != null)
            StopCoroutine(_requestCoroutine);
        _requestCoroutine = StartCoroutine(RequestTimerRoutine());
    }
    
    private IEnumerator RequestTimerRoutine()
    {
        float timer = _currentRequestTimeout; // Используем динамическое значение
        
        while (timer > 0 && _isWaitingForTea)
        {
            timer -= Time.deltaTime;
            _visitorUI?.SetWaitTimer(_currentRequestTimeout, timer);
            yield return null;
        }
        
        if (_isWaitingForTea)
        {
            _visitorUI?.ShowMessage("The spirit got tired of waiting...");
            RejectOrder();
        }
    }
    
    private void AcceptOrder()
    {
        if (!_isWaitingForResponse) return;
        
        if (_responseCoroutine != null)
            StopCoroutine(_responseCoroutine);
        
        _isWaitingForResponse = false;
        _isWaitingForTea = true;
        
        _visitorUI?.ShowOrderUI(true);
        _visitorUI?.ShowMessage("Приготовь заказ!");
        _visitorUI?.HideButton();
        StartRequestTimer();
    }
    
    private void CheckBrewedTea()
    {
        TeaData lastBrewed = _teaMaker?.GetLastBrewedTea();
        if (lastBrewed == null) return;
        
        _teaMaker.ResetLastBrewedTea();
        
        if (_orderMatcher.TrySubmitTea(lastBrewed))
        {
            _visitorUI?.UpdateRequestDisplay(_orderMatcher.CurrentOrder);
        }
    }
    
    private void OnOrderCompleted()
    {
        _rewardDistributor.GiveReward(_orderMatcher.CurrentOrder);
        _visitorUI?.ShowMessage($"The spirit is pleased! + droplets");
        _visitorUI?.ShowOrderUI(true);
        EndVisit();
    }
    
    private void RejectOrder()
    {
        _visitorUI?.ShowMessage("The spirit left...");
        _visitorUI?.ShowOrderUI(false);
        EndVisit();
    }
    
    private void EndVisit()
    {
        hint.gameObject.SetActive(false);
        _isWaitingForResponse = false;
        _isWaitingForTea = false;
        
        if (_responseCoroutine != null)
            StopCoroutine(_responseCoroutine);
        if (_requestCoroutine != null)
            StopCoroutine(_requestCoroutine);
        
        _orderMatcher.ClearOrder();
        _visitorUI?.HideVisitor();
        _visitorUI?.ResetUI();
        
        ScheduleNextVisit();
        ResetTimeouts();
    }
    
    public void ForceVisit()
    {
        if (!_isWaitingForResponse && !_isWaitingForTea)
        {
            _timeUntilNextVisit = 0;
        }
    }
}