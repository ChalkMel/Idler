using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Credits : MonoBehaviour
{
  [Header("Credits")]
  [SerializeField] public int droplets = 0;
  [SerializeField] public int leaves = 0;
  [SerializeField] public int berries = 0;
  [SerializeField] public int flowers = 0;
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
  public int dropletsMulti = 1;
  public int leavesMulti = 1;
  public int berriesMulti = 1;
  public int flowersMulti = 1;

  private System.Random _random;

  private void Awake()
  {
    _random = new System.Random();
  }

  public void BushDrop()
  {
    leaves += _random.Next(_leavesChance[0] * leavesMulti, (_leavesChance[1] + 1) * leavesMulti);
    int random = _random.Next(1, 3);
    switch (random)
    {
      case 1:
        berries += _random.Next(_berriesChance[0] * berriesMulti, (_berriesChance[1] + 1) * berriesMulti);
        break;
      case 2:
        flowers += _random.Next(_flowersChance[0] * flowersMulti, (_flowersChance[1] + 1) * flowersMulti);
        break;
    }
    UpdateUI();
  }

  public void DropletsDrop()
  {
    droplets += _random.Next(_dropletsChance[0] * dropletsMulti, (_dropletsChance[1] + 1) * dropletsMulti);
    UpdateUI();
  }
    
  private void UpdateUI()
  {
    _dropletsText.text = $"{droplets} of droplets";
    _leavesText.text = $"{leaves} of leaves";
    _berriesText.text = $"{berries} of berries";
    _flowersText.text = $"{flowers} of flowers";
  }
}