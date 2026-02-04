// SpiritMenuButton.cs
using UnityEngine;
using UnityEngine.UI;

public class SpiritMenuToggle : MonoBehaviour
{
  [SerializeField] private SpiritMenu spiritMenu;
    
  private void Start()
  {
    GetComponent<Button>().onClick.AddListener(ToggleMenu);
  }
    
  private void ToggleMenu()
  {
    if (spiritMenu.gameObject.activeSelf)
    {
      spiritMenu.CloseMenu();
    }
    else
    {
      spiritMenu.OpenMenu();
    }
  }
}