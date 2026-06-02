namespace Save
{
  using UnityEngine;
  using UnityEngine.UI;

  public class SaveButton : MonoBehaviour
  {
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button deleteButton;
    private void Start()
    {
      saveButton.onClick.AddListener(Save);
      deleteButton.onClick.AddListener(Delete);
    }
    
    private void Save()
    {
      saveManager.SaveGame();
      Debug.Log("Manual save");
    }
    
    private void Delete()
    {
      saveManager.DeleteSave();
      Debug.Log("Save deleted");
    }
  }
}