using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
public class Cloud : MonoBehaviour
{
    [SerializeField] private Credits credits;
    [SerializeField] private GameObject dropletPrefab;
    [SerializeField] private Vector2 punchScale;
    [SerializeField] private float duration;
    [SerializeField] private float rangeY = 2f;
    [SerializeField] private float rangeX = -0.7f;
    private void OnMouseDown()
    {
        if(EventSystem.current.IsPointerOverGameObject()) return;
        credits.DropletsDrop();
        transform.DOPunchScale(punchScale, duration);
        
        Vector2 randomPoint = (Vector2)transform.position + 
                              new Vector2(Random.Range(rangeY, -rangeY), rangeX);
        
        GameObject droplet = Instantiate(dropletPrefab, randomPoint, Quaternion.identity);

        droplet.transform.DOMoveY(droplet.transform.position.y -10f, 0.5f)
            .SetEase(Ease.InCubic);
        if (droplet.transform != null)
            Destroy(droplet, 0.55f);
    }
}
