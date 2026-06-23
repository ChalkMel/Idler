using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VisitorSpawner : MonoBehaviour
{
    [SerializeField] private OrderGenerator orderGenerator;
    [SerializeField] private OrderMatcher orderMatcher;
    [SerializeField] private RewardDistributor rewardDistributor;
    [SerializeField] private VisitorUI visitorUI;
    [SerializeField] private TeaBrewingController teaMaker;
    
    [Header("Timing")]
    [SerializeField] private float minTimeBetweenVisits = 30f;
    [SerializeField] private float maxTimeBetweenVisits = 60f;
    [SerializeField] private float responseTimeout = 15f;
    [SerializeField] private float requestTimeout = 30f;
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
        _baseResponseTimeout = responseTimeout;
        _baseRequestTimeout = requestTimeout;
        
        ResetTimeouts();
        
        _audioSource = GetComponent<AudioSource>();
        ScheduleNextVisit();
        
        if (visitorUI != null)
        {
            visitorUI.AcceptButton.onClick.AddListener(AcceptOrder);
            visitorUI.RejectButton.onClick.AddListener(RejectOrder);
        }
        
        if (orderMatcher != null)
        {
            orderMatcher.OnOrderCompleted += OnOrderCompleted;
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
            _currentResponseTimeout = Mathf.Max(1f, _currentResponseTimeout);
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
            visitorUI?.SetNextVisitTimer(_timeUntilNextVisit, maxTimeBetweenVisits);
        
            if (_timeUntilNextVisit <= 0)
            {
                SpawnVisitor();
            }
        }
    
        if (_isWaitingForTea && teaMaker != null)
        {
            CheckBrewedTea();
        }
    }
    
    private void ScheduleNextVisit()
    {
        _timeUntilNextVisit = Random.Range(minTimeBetweenVisits, maxTimeBetweenVisits);
    }
    
    private void SpawnVisitor()
    {
        hint.gameObject.SetActive(true);
        _audioSource.Play();
        OrderData order = orderGenerator.GenerateOrder();
        if (order == null) return;
        
        orderMatcher.SetOrder(order);
        visitorUI.ShowVisitor(order.visitor, order.GetOrderText());
        
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
        float timer = _currentResponseTimeout;
        
        while (timer > 0 && _isWaitingForResponse)
        {
            timer -= Time.deltaTime;
            visitorUI?.SetResponseTimer(_currentResponseTimeout, timer);
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
        float timer = _currentRequestTimeout;
        
        while (timer > 0 && _isWaitingForTea)
        {
            timer -= Time.deltaTime;
            visitorUI?.SetWaitTimer(_currentRequestTimeout, timer);
            yield return null;
        }
        
        if (_isWaitingForTea)
        {
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
        
        visitorUI?.ShowOrderUI(true);
        visitorUI?.HideButton();
        StartRequestTimer();
    }
    
    private void CheckBrewedTea()
    {
        TeaData lastBrewed = teaMaker?.GetLastBrewedTea();
        if (lastBrewed == null) return;
        
        teaMaker.ResetLastBrewedTea();
        
        if (orderMatcher.TrySubmitTea(lastBrewed))
        {
            visitorUI?.UpdateRequestDisplay(orderMatcher.CurrentOrder);
        }
    }
    
    private void OnOrderCompleted()
    {
        rewardDistributor.GiveReward(orderMatcher.CurrentOrder);
        visitorUI?.ShowOrderUI(true);
        EndVisit();
    }
    
    private void RejectOrder()
    {
        visitorUI?.ShowOrderUI(false);
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
        
        orderMatcher.ClearOrder();
        visitorUI?.HideVisitor();
        visitorUI?.ResetUI();
        
        ScheduleNextVisit();
        ResetTimeouts();
    }
}