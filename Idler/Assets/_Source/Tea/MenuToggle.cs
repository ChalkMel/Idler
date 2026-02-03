using UnityEngine;
using UnityEngine.UI;

public class MenuToggle : MonoBehaviour
{
  [SerializeField] private TeaMenu teaMenu;
    
  private void Start()
  {
    Button button = GetComponent<Button>();
    if (button != null && teaMenu != null)
    {
      button.onClick.AddListener(ToggleMenu);
    }
  }
    
  private void ToggleMenu()
  {
    if (teaMenu.gameObject.activeSelf)
    {
      teaMenu.CloseMenu();
    }
    else
    {
      teaMenu.OpenMenu();
    }
  }
}