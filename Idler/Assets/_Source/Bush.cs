using UnityEngine;

public class Bush : MonoBehaviour
{
    [SerializeField] private BushGenerator generator;
    
    public void Initialize(BushGenerator gen)
    {
        generator = gen;
    }
    
    private void OnMouseDown()
    {
       var collider = GetComponent<Collider2D>();
       collider.enabled = false;
      generator.OnBushClicked(this);
        
    }
}
