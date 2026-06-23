using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DraggableIng : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private IngredientData ingredientData;
    [SerializeField] private TeaBrewingController teaController;
    [SerializeField] private string dropZoneTag = "DropZone";
    [SerializeField] private AudioSource audioSource;
    private Vector2 _originalPosition;
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    
    private void Awake()
    {
        GetComponent<Image>().sprite = ingredientData.Icon;
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            
        _originalPosition = _rectTransform.anchoredPosition;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = 0.6f;
        _canvasGroup.blocksRaycasts = false;
        transform.localScale = Vector3.one * 1.1f;
        audioSource.Play();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        audioSource.Stop();
        
        bool droppedOnCauldron = IsDroppedOnCauldron(eventData);
        
        if (droppedOnCauldron && teaController != null && ingredientData != null)
        {
            teaController.AddIngredient(ingredientData);
        }
        
        _rectTransform.anchoredPosition = _originalPosition;
    }
    
    private bool IsDroppedOnCauldron(PointerEventData eventData)
    {
        if (eventData.pointerEnter == null) return false;
        
        if (eventData.pointerEnter.CompareTag(dropZoneTag))
            return true;
        
        Transform parentCheck = eventData.pointerEnter.transform;
        while (parentCheck != null)
        {
            if (parentCheck.CompareTag(dropZoneTag))
                return true;
            parentCheck = parentCheck.parent;
        }
        
        return false;
    }
}