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
  public int HelperCount;
  public int DropletHelperCount;
  
  public float helperCollectionInterval = 5f;
  public float DropletHelperCollectionInterval = 5f;
  
  private float _helperTimer;
  private float _baseHelperInterval;
  private float _dropletHelperTimer;
  private float _dropletBaseHelperInterval;
  
  [Header("Chances")]
  [SerializeField] private List<int> dropletsChance  = new List<int>(2);
  [SerializeField] private List<int> leavesChance  = new List<int>(2);
  [SerializeField] private List<int> berriesChance  = new List<int>(2);
  [SerializeField] private List<int> flowersChance  = new List<int>(2);
  
  [Header("UI")]
  [SerializeField] private TextMeshProUGUI dropletsText;
  [SerializeField] private TextMeshProUGUI leavesText;
  [SerializeField] private TextMeshProUGUI berriesText;
  [SerializeField] private TextMeshProUGUI flowersText;
  public float DropletsMulti = 1;
  public float LeavesMulti = 1;
  public float BerriesMulti = 1;
  public float FlowersMulti = 1;

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
    if (HelperCount == 0 && DropletHelperCount == 0) return;
    if (HelperCount != 0)
    {
      _helperTimer += Time.deltaTime;
      if (_helperTimer >= helperCollectionInterval)
      {
        int random = _random.Next(0, 3);
        switch (random)
        {
          case 0:
            flowers += (int) Mathf.Round(HelperCount * FlowersMulti);
            break;
          case 1:
            leaves += (int) Mathf.Round(HelperCount * LeavesMulti);
            break;
          case 2:
            berries += (int) Mathf.Round(HelperCount * BerriesMulti);
            break;
        }

        _helperTimer = 0;
        UpdateUI();
      }
    }

    if (DropletHelperCount != 0)
    {
      _dropletHelperTimer += Time.deltaTime;
      if (_dropletHelperTimer >= DropletHelperCollectionInterval)
      {
        droplets += (int) Mathf.Round(DropletHelperCount * DropletsMulti);
        _dropletHelperTimer = 0;
        UpdateUI();
      }
    }
  }

  public void BushDrop()
  {
    leaves += _random.Next((int) (leavesChance[0] * LeavesMulti), (int) ((leavesChance[1] + 1) * LeavesMulti));
    int random = _random.Next(1, 3);
    switch (random)
    {
      case 1:
        berries += _random.Next((int) (berriesChance[0] * BerriesMulti), (int) ((berriesChance[1] + 1) * BerriesMulti));
        break;
      case 2:
        flowers += _random.Next((int) (flowersChance[0] * FlowersMulti), (int) ((flowersChance[1] + 1) * FlowersMulti));
        break;
    }
    UpdateUI();
  }

  public void DropletsDrop()
  {
    droplets += _random.Next((int) (dropletsChance[0] * DropletsMulti), (int) ((dropletsChance[1] + 1) * DropletsMulti));
    UpdateUI();
  }
    
  public void UpdateUI()
  {
    dropletsText.text = $"{droplets}";
    leavesText.text = $"{leaves}";
    berriesText.text = $"{berries}";
    flowersText.text = $"{flowers}";
  }
}