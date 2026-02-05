using UnityEngine;
using DG.Tweening;
public class Cloud : MonoBehaviour
{
    [SerializeField] private Credits credits;
    [SerializeField] private GameObject dropletPrefab;
    [SerializeField] private Vector2 punchScale;
    [SerializeField] private float duration;
    private void OnMouseDown()
    {
        credits.DropletsDrop();
        transform.DOPunchScale(punchScale, duration);
        
        Vector2 randomPoint = (Vector2)transform.position + 
                              new Vector2(Random.Range(-2f, 2f), -0.7f);
        
        GameObject droplet = Instantiate(dropletPrefab, randomPoint, Quaternion.identity);

        droplet.transform.DOMoveY(droplet.transform.position.y -10f, 0.5f)
            .SetEase(Ease.InCubic);
        if (droplet.transform != null)
            Destroy(droplet, 0.55f);
    }
}
