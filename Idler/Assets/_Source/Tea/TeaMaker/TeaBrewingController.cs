using System.Collections.Generic;
using UnityEngine;

public class TeaBrewingController : MonoBehaviour
{
    
    [Header("References")]
    [SerializeField] private Credits _credits;
    [SerializeField] private SpiritBuffManager _buffManager;
    [SerializeField] private SpiritVisitor _spiritVisitor;
    [SerializeField] private SpiritCollection _playerSpirits;

    [Header("Tea Recipes")]
    [SerializeField] public List<TeaData> allTeas = new List<TeaData>();

    [Header("UI")]
    [SerializeField] private CauldronUI _cauldronUI;
    [SerializeField] private TeaResultPresenter _resultPresenter;

    [Header("Services")]
    [SerializeField] private IngredientInventory _inventory;
    [SerializeField] private BrewingProcess _brewingProcess;
    private TeaRecipeMatcher _recipeMatcher = new TeaRecipeMatcher();

    private List<IngredientData> _currentIngredients = new List<IngredientData>();
    private TeaData _lastBrewedTea;

    private void Awake()
    {
        if (_inventory == null) _inventory = GetComponent<IngredientInventory>();
        if (_brewingProcess == null) _brewingProcess = GetComponent<BrewingProcess>();
        if (_cauldronUI == null) _cauldronUI = GetComponent<CauldronUI>();

        if (_cauldronUI != null)
        {
            _cauldronUI.BrewButton.onClick.AddListener(StartBrewing);
            _cauldronUI.ClearButton.onClick.AddListener(ClearCauldron);
        }

        _brewingProcess.OnBrewingCompleted.AddListener(OnBrewingCompleted);
    }

    private void Start()
    {
        foreach (var tea in allTeas)
        {
            if (tea.baseBrewingTime == 0)
                tea.baseBrewingTime = tea.brewingTime;
        }
        
        UpdateCounters();
        _cauldronUI?.SetClearButtonInteractable(true);
    }

    private void Update()
    {
        UpdateBrewButtonState();
    }

    public void AddIngredient(IngredientData ingredient)
    {
        
        if (_brewingProcess.IsBrewing)
        {
            ShowMessage("Дождитесь!");
            return;
        }

        if (_buffManager != null && _buffManager.GetActiveSpiritsCount() == _buffManager.MaxSpiritSlots && !_spiritVisitor.IsWaitingForTea)
        {
            ShowMessage("Дождитесь!");
            return;
        }

        if (_spiritVisitor.IsWaitingForTea)
        {
            ShowMessage(_spiritVisitor.SetOrderText());
        }

        if (!_inventory.HasIngredient(ingredient))
        {
            ShowMessage($"У вас нет {ingredient.ingredientName}!");
            return;
        }

        if (_currentIngredients.Count >= 6)
        {
            ShowMessage("Максимум 6!");
            return;
        }

        _currentIngredients.Add(ingredient);
        _inventory.UseIngredient(ingredient);
        _cauldronUI?.AddIngredientIcon(ingredient);
        UpdateCounters();
        
        Debug.Log($"Added ingredient {ingredient.ingredientName}, total: {_currentIngredients.Count}");
    }

    public void UpdateCounters()
    {
        _cauldronUI?.UpdateCounters(_credits);
    }

    private void StartBrewing()
    {
        if (_brewingProcess.IsBrewing)
        {
            ShowMessage("Уже в работе!");
            return;
        }

        if (_buffManager != null && _buffManager.GetActiveSpiritsCount() == _buffManager.MaxSpiritSlots && !_spiritVisitor.IsWaitingForTea)
        {
            ShowMessage("Нельзя варить пока нет мест!");
            return;
        }

        if (_currentIngredients.Count == 0)
        {
            ShowMessage("Добавь что-то!");
            return;
        }

        TeaData matchedTea = _recipeMatcher.FindMatchingTea(_currentIngredients, allTeas);
        if (matchedTea == null)
        {
            ShowMessage("Что-то пошло не так!");
            ReturnIngredientsAndClear();
            return;
        }

        _cauldronUI?.SetBrewButtonInteractable(false);
        _cauldronUI?.SetClearButtonInteractable(false);
        
        _brewingProcess.StartBrewing(matchedTea);
    }

    private void OnBrewingCompleted(TeaData tea)
    {
        Debug.Log("Brewing completed! Cleaning up...");
        
        _lastBrewedTea = tea;

        if (_spiritVisitor.IsWaitingForTea)
        {
            
        }
        else
        {
            List<SpiritData> likedSpirits = tea.GetLikedSpiritsForPlayer(_playerSpirits);
            if (likedSpirits.Count > 0)
            {
                SpiritData chosenSpirit = likedSpirits[Random.Range(0, likedSpirits.Count)];
                Debug.Log("Here is result");
                _resultPresenter?.ShowResult(tea, chosenSpirit);
                ApplySpiritBuff(chosenSpirit);
                GiveReward(chosenSpirit);
                ShowMessage($"Ура! сварили {tea.teaName} и пришел дух {chosenSpirit.spiritName}");
            }
            else
            {
                ShowMessage("Пока этот чай никому не понравился!");
                ReturnIngredientsAndClear();
                _cauldronUI?.SetBrewButtonInteractable(true);
                _cauldronUI?.SetClearButtonInteractable(true);
                return;
            }
        }
        
        ClearAllAfterBrewing();
    }

    private void ClearAllAfterBrewing()
    {
        Debug.Log("ClearAllAfterBrewing called");
        
        _currentIngredients.Clear();
        
        if (_cauldronUI != null)
        { 
            Debug.Log("ClearAllAfterBrewing called");
            _cauldronUI.ClearIngredientIcons();
        }
        else
        {
            Debug.LogError("CauldronUI is null!");
        }
        
        UpdateCounters();
        
        if (_cauldronUI != null)
        {
            _cauldronUI.SetBrewButtonInteractable(true);
            _cauldronUI.SetClearButtonInteractable(true);
        }
        
        Debug.Log($"Current ingredients count after clear: {_currentIngredients.Count}");
    }

    private void ApplySpiritBuff(SpiritData spirit)
    {
        if (_buffManager != null && spirit != null)
            _buffManager.AddBuff(spirit);
    }

    private void GiveReward(SpiritData spirit)
    {
        if (_credits == null || spirit == null) return;
        int baseReward = 10;
        int finalReward = Mathf.RoundToInt(baseReward * spirit.buffMultiplier);
        _credits.droplets += finalReward;
        _credits.UpdateUI();
    }

    private void ClearCauldron()
    {
        if (_brewingProcess.IsBrewing)
        {
            return;
        }
        
        Debug.Log("ClearCauldron called manually");
        ReturnIngredientsAndClear();
        ClearVisuals();
        _cauldronUI?.SetClearButtonInteractable(true);
    }

    private void ReturnIngredientsAndClear()
    {
        _inventory.ReturnIngredients(_currentIngredients);
        _currentIngredients.Clear();
        ClearVisuals();
        UpdateCounters();
    }

    private void ClearVisuals()
    {
        if (_cauldronUI != null)
        {
            _cauldronUI.ClearIngredientIcons();
        }
        _currentIngredients.Clear();
        UpdateCounters();
    }

    private void UpdateBrewButtonState()
    {
        if (_cauldronUI?.BrewButton == null) return;
        bool canBrew = !_brewingProcess.IsBrewing && _currentIngredients.Count > 0;
        _cauldronUI.SetBrewButtonInteractable(canBrew);
    }

    private void ShowMessage(string message)
    {
        _cauldronUI?.ShowMessage(message);
    }

    public TeaData GetLastBrewedTea()
    {
        return _lastBrewedTea;
    }

    public void ResetLastBrewedTea()
    {
        _lastBrewedTea = null;
    }
}