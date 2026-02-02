using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] private Credits credits;

    private void OnMouseDown()
    {
        credits.DropletsDrop();
    }
}
