using UnityEngine;
using UnityEngine.UI;

public class SpiritDebugSpawn : MonoBehaviour
{
  [Header("Debug Settings")]
  [SerializeField] private SpiritBuffManager buffManager;
  [SerializeField] private SpiritData spiritToSpawn;
    
  [Header("Optional - Button")]
  [SerializeField] private Button spawnButton;
    
  private void Start()
  {
    // Если есть кнопка, привязываем к ней
    if (spawnButton != null)
    {
      spawnButton.onClick.AddListener(SpawnSpirit);
    }
  }
    
  // Метод для спавна духа (можно вызывать из других скриптов)
  public void SpawnSpirit()
  {
    if (buffManager == null)
    {
      Debug.LogError("SpiritBuffManager not assigned!");
      return;
    }
        
    if (spiritToSpawn == null)
    {
      Debug.LogError("SpiritData not assigned!");
      return;
    }
        
    Debug.Log($"Spawning spirit: {spiritToSpawn.spiritName}");
    buffManager.AddBuff(spiritToSpawn);
  }
    
  // Метод для спавна конкретного духа (для вызова из инспектора)
  public void SpawnSpecificSpirit(SpiritData spirit)
  {
    if (buffManager == null)
    {
      Debug.LogError("SpiritBuffManager not assigned!");
      return;
    }
        
    if (spirit == null)
    {
      Debug.LogError("SpiritData is null!");
      return;
    }
        
    Debug.Log($"Spawning spirit: {spirit.spiritName}");
    buffManager.AddBuff(spirit);
  }
    
  // Для удобства: кнопки в инспекторе
  [ContextMenu("Spawn Assigned Spirit")]
  private void SpawnAssignedSpirit()
  {
    SpawnSpirit();
  }
}