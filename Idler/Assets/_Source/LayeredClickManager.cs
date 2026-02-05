using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class LayeredClickManager : MonoBehaviour
{
  [SerializeField] private GraphicRaycaster uiRaycaster;
  [SerializeField] private LayerMask uiLayer;
  [SerializeField] private LayerMask worldLayer;
    
  private EventSystem eventSystem;
    
  private void Start()
  {
    eventSystem = EventSystem.current;
  }
    
  private void Update()
  {
    if (Input.GetMouseButtonDown(0))
    {
      HandleClick();
    }
  }
    
  private void HandleClick()
  {
    // Сначала проверяем UI
    PointerEventData pointerData = new PointerEventData(eventSystem)
    {
      position = Input.mousePosition
    };
        
    List<RaycastResult> results = new List<RaycastResult>();
    uiRaycaster.Raycast(pointerData, results);
        
    if (results.Count > 0)
    {
      // Клик по UI - обрабатываем
      foreach (var result in results)
      {
        Debug.Log("UI clicked: " + result.gameObject.name);
        // Ваша логика для UI
      }
      return; // Не проверяем мир дальше
    }
        
    // Если UI не кликнут, проверяем 2D мир
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, worldLayer);
        
    if (hit.collider != null)
    {
      Debug.Log("2D world object clicked: " + hit.collider.name);
      // Ваша логика для 2D объектов
    }
  }
}