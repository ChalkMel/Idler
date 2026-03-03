using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableIng : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
  public IngredientData ingredientData;
  [SerializeField] private Transform parent;
  [SerializeField] private TeaMaker teaMaker;
  private Vector2 _originalPosition;
  private RectTransform _rectTransform;
  private Canvas _canvas;
  [SerializeField] private string _dropZoneTag = "DropZone";

  private void Awake()
  {
    _rectTransform = GetComponent<RectTransform>();
    _canvas = GetComponentInParent<Canvas>();
    _originalPosition = _rectTransform.anchoredPosition;
  }

  public void OnBeginDrag(PointerEventData eventData)
  {
    transform.localScale = Vector3.one * 1.1f;
  }

  public void OnDrag(PointerEventData eventData)
  {
    _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
  }

  public void OnEndDrag(PointerEventData eventData)
  {
    transform.localScale = Vector3.one;
    
    if (!eventData.pointerEnter || !eventData.pointerEnter.CompareTag(_dropZoneTag))
    {
      AddIng();
    }
    _rectTransform.anchoredPosition = _originalPosition;
  }

  private void AddIng()
  {
    teaMaker.AddIngredient(ingredientData);
  }
}