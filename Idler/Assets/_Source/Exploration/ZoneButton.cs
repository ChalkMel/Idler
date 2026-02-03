using UnityEngine;
using UnityEngine.UI;
public class ZoneButton : MonoBehaviour
{
    [SerializeField] private int zoneIndex;
    [SerializeField] private Exploration explorationManager;
    private Button _button;
    
    private void Start()
    {
        _button = GetComponent<Button>();
            
        _button.onClick.AddListener(OnClick);
    }
    
    private void OnClick()
    {
      explorationManager.SelectZone(zoneIndex);
    }
}
