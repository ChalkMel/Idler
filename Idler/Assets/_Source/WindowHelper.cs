using UnityEngine;
using DG.Tweening;

public static class WindowHelper
{
  private const float DURATION = 0.25f;
  
  public static void Show(CanvasGroup window, float duration)
  {
    window.gameObject.SetActive(true);
    window.transform.localScale = Vector3.zero;
    window.alpha = 0f;
    if (duration == 0)
      window.transform.DOScale(1f, DURATION).SetEase(Ease.OutBack);
    else
      window.transform.DOScale(1f, duration).SetEase(Ease.OutBack);
    window.DOFade(1f, duration*4).SetEase(Ease.OutBack);
  }
  
  public static void Hide(CanvasGroup window)
  {
    window.transform.DOScale(0f, DURATION).SetEase(Ease.InBack).OnComplete(() =>
    {
      window.gameObject.SetActive(false);
    });
  }
  
  public static void ShowWithSlide(CanvasGroup window, float duration, Vector2 startOffset)
  {
    window.gameObject.SetActive(true);
    RectTransform rect = window.GetComponent<RectTransform>();
    Vector2 originalPos = rect.anchoredPosition;
    
    rect.anchoredPosition = originalPos + startOffset;
    //window.transform.localScale = Vector3.zero;
    window.alpha = 0f;
    
    float animDuration = duration == 0 ? DURATION : duration;
    rect.DOAnchorPos(originalPos, animDuration).SetEase(Ease.OutCubic);
    //window.transform.DOScale(1f, animDuration).SetEase(Ease.OutBack);
    window.DOFade(1f, animDuration * 4).SetEase(Ease.OutBack);
  }

  public static void HideWithSlide(CanvasGroup window, float duration, Vector2 slideDirection)
  {
    if (window == null || !window.gameObject.activeSelf) return;
    
    RectTransform rect = window.GetComponent<RectTransform>();
    Vector2 targetPos = rect.anchoredPosition + slideDirection;
    float animDuration = duration == 0 ? DURATION : duration;
    
    rect.DOAnchorPos(targetPos, animDuration).SetEase(Ease.InQuad);
   // window.transform.DOScale(0f, animDuration).SetEase(Ease.InBack);
    window.DOFade(0f, animDuration).OnComplete(() =>
    {
      window.gameObject.SetActive(false);
      
      rect.anchoredPosition = rect.anchoredPosition - slideDirection;
    });
  }
}