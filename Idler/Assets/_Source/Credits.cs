using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class Credits : MonoBehaviour
{
  [Header("Credits")]
  [SerializeField] public int droplets;
  [SerializeField] public int leaves;
  [SerializeField] public int berries;
  [SerializeField] public int flowers;
  
  [Header("Helpers")]
  public int HelperCount = 0;
  public int DropletHelperCount = 0;
  
  public float helperCollectionInterval = 5f;
  public float DropletHelperCollectionInterval = 5f;
  
  private float _helperTimer;
  private float _baseHelperInterval;
  private float _DropletHelperTimer;
  private float _DropletBaseHelperInterval;
  
  [Header("Chances")]
  [SerializeField] private List<int> _dropletsChance  = new List<int>(2);
  [SerializeField] private List<int> _leavesChance  = new List<int>(2);
  [SerializeField] private List<int> _berriesChance  = new List<int>(2);
  [SerializeField] private List<int> _flowersChance  = new List<int>(2);
  [Header("UI")]
  [SerializeField] private TextMeshProUGUI _dropletsText;
  [SerializeField] private TextMeshProUGUI _leavesText;
  [SerializeField] private TextMeshProUGUI _berriesText;
  [SerializeField] private TextMeshProUGUI _flowersText;
  public float dropletsMulti = 1;
  public float leavesMulti = 1;
  public float berriesMulti = 1;
  public float flowersMulti = 1;

  private Random _random;
  private float _timer;
  private void Awake()
  {
    _baseHelperInterval = helperCollectionInterval;
    UpdateUI();
    _random = new Random();
  }

  private void Update()
  {
    if (HelperCount == 0 || DropletHelperCount == 0) return;
        
    _helperTimer += Time.deltaTime;
    if (_helperTimer >= helperCollectionInterval)
    {
      int random = _random.Next(0, 3);
      switch (random)
      {
        case 0:
          flowers += (int)Mathf.Round(HelperCount * flowersMulti);
          break;
        case 1:
          leaves += (int)Mathf.Round(HelperCount * leavesMulti);
          break;
        case 2:
          berries += (int)Mathf.Round(HelperCount * berriesMulti);
          break;
      }
      _helperTimer = 0;
      UpdateUI();
    }
    
    _DropletHelperTimer += Time.deltaTime;
    if (_DropletHelperTimer >= helperCollectionInterval)
    {
      droplets += (int)Mathf.Round(DropletHelperCount * dropletsMulti);
      _DropletHelperTimer = 0;
      UpdateUI();
    }
  }
  public void BushDrop()
  {
    leaves += _random.Next((int) (_leavesChance[0] * leavesMulti), (int) ((_leavesChance[1] + 1) * leavesMulti));
    int random = _random.Next(1, 3);
    switch (random)
    {
      case 1:
        berries += _random.Next((int) (_berriesChance[0] * berriesMulti), (int) ((_berriesChance[1] + 1) * berriesMulti));
        break;
      case 2:
        flowers += _random.Next((int) (_flowersChance[0] * flowersMulti), (int) ((_flowersChance[1] + 1) * flowersMulti));
        break;
    }
    UpdateUI();
  }

  public void DropletsDrop()
  {
    droplets += _random.Next((int) (_dropletsChance[0] * dropletsMulti), (int) ((_dropletsChance[1] + 1) * dropletsMulti));
    UpdateUI();
  }
    
  public void UpdateUI()
  {
    _dropletsText.text = $"{droplets} of droplets";
    _leavesText.text = $"{leaves} of leaves";
    _berriesText.text = $"{berries} of berries";
    _flowersText.text = $"{flowers} of flowers";
  }
}