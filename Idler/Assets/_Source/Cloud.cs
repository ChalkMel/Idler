using UnityEngine;
using DG.Tweening;
public class Cloud : MonoBehaviour
{
    [SerializeField] private Credits credits;
    [SerializeField] private Vector2 punchScale;
    [SerializeField] private float duration;
    private void OnMouseDown()
    {
        credits.DropletsDrop();
        transform.DOPunchScale(punchScale, duration);
    }
}
