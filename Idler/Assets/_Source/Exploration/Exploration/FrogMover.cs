using System.Collections;
using UnityEngine;
using DG.Tweening;

public class FrogMover : MonoBehaviour
{
  [SerializeField] private GameObject _frogSprite;
  [SerializeField] private GameObject _frogMover;
  [SerializeField] private Vector3 _originalPosition;
    
  public event System.Action OnMovementStarted;
  public event System.Action OnMovementCompleted;
    
  private void Awake()
  {
    if (_frogMover != null)
      _originalPosition = _frogMover.transform.position;
  }
    
  public IEnumerator MoveToTargetAndBack(Transform target)
  {
    if (_frogMover == null || target == null) yield break;
        
    _frogMover.gameObject.SetActive(true);
        
    if (_frogSprite != null)
    {
      _frogSprite.transform.DOPunchScale(new Vector2(0.5f, 0.5f), 0.2f);
      _frogSprite.gameObject.SetActive(false);
    }
        
    OnMovementStarted?.Invoke();
        
    float moveDuration = 0.5f;
    
    _frogMover.transform.DOMove(target.position, moveDuration).SetEase(Ease.OutQuad);
    yield return new WaitForSeconds(moveDuration);
    
    yield return new WaitForSeconds(10f); //TODO добавить время
    
    _frogMover.transform.DOMove(_originalPosition, moveDuration).SetEase(Ease.InQuad);
    yield return new WaitForSeconds(moveDuration);
    
    _frogMover.gameObject.SetActive(false);
    if (_frogSprite != null)
    {
      _frogSprite.gameObject.SetActive(true);
      _frogSprite.transform.DOPunchScale(new Vector2(0.5f, 0.5f), 0.2f);
    }
        
    OnMovementCompleted?.Invoke();
  }
    
  public void ResetPosition()
  {
    if (_frogMover != null)
      _frogMover.transform.position = _originalPosition;
  }
}