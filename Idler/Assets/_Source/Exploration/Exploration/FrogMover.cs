using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

public class FrogMover : MonoBehaviour
{
    [SerializeField] private GameObject frogSprite;
    [SerializeField] private GameObject frogMover;
    [SerializeField] private Vector3 originalPosition;
    [SerializeField] private GameObject panel;
    
    public event System.Action OnMovementStarted;
    public event System.Action OnMovementCompleted;
    
    private bool _isMoving = false;
    
    private void Awake()
    {
        if (frogMover != null)
            originalPosition = frogMover.transform.position;
    }
    
    public IEnumerator MoveToTargetAndBack(Transform target)
    {
        if (frogMover == null || target == null || _isMoving) yield break;
        
        _isMoving = true;
        frogMover.gameObject.SetActive(true);
        panel.SetActive(false);
        
        if (frogSprite != null)
        {
            frogSprite.transform.DOPunchScale(new Vector2(0.5f, 0.5f), 0.2f);
            frogSprite.gameObject.SetActive(false);
        }
        
        OnMovementStarted?.Invoke();
        
        float moveDuration = 0.5f;
        
        frogMover.transform.DOMove(target.position, moveDuration).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(moveDuration);
        
        yield return new WaitForSeconds(10f);
        
        frogMover.transform.DOMove(originalPosition, moveDuration).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(moveDuration);
        
        panel.SetActive(true);
        frogMover.gameObject.SetActive(false);
        
        if (frogSprite != null)
        {
            frogSprite.gameObject.SetActive(true);
            frogSprite.transform.DOPunchScale(new Vector2(0.5f, 0.5f), 0.2f);
        }
        
        _isMoving = false;
        OnMovementCompleted?.Invoke();
    }
    
    public void ResetPosition()
    {
        if (frogMover != null)
            frogMover.transform.position = originalPosition;
        _isMoving = false;
    }
    
    public bool IsMoving => _isMoving;
}