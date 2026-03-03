using UnityEngine;
using UnityEngine.EventSystems;

public class Bush : MonoBehaviour
{
    [SerializeField] private BushGenerator generator;
    
    public void Initialize(BushGenerator gen)
    {
        generator = gen;
    }
    
    private void OnMouseDown()
    { 
        if(EventSystem.current.IsPointerOverGameObject()) return; 
        var collider = GetComponent<Collider2D>(); 
        GetComponent<Collider2D>().enabled = false; 
        generator.OnBushClicked(this);
        
    }
}
