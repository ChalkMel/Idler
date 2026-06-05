// TutorialManager.cs
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Sprite[] slides;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Image slideImage;
    [SerializeField] private Button closeButton;
    
    private int currentSlide = 0;
    private const string TUTORIAL_COMPLETED_KEY = "TutorialCompleted";
    
    private void Start()
    {
        closeButton.onClick.AddListener(CloseTutorial);
        
        if (IsTutorialCompleted())
        {
            tutorialPanel.SetActive(false);
        }
        else
        {
            ShowTutorial();
        }
    }
    
    public bool IsTutorialCompleted()
    {
        return PlayerPrefs.GetInt(TUTORIAL_COMPLETED_KEY) == 1;
    }
    
    public void SetTutorialCompleted()
    {
        PlayerPrefs.SetInt(TUTORIAL_COMPLETED_KEY, 1);
        PlayerPrefs.Save();
        tutorialPanel.SetActive(false);
    }
    
    public void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        currentSlide = 0;
        UpdateSlide();
    }
    
    private void UpdateSlide()
    {
        if (slides.Length == 0) return;
        slideImage.sprite = slides[currentSlide];
    }
    
    private void Update()
    {
        if (!tutorialPanel.activeSelf) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            NextSlide();
        }
    }
    
    private void NextSlide()
    {
        if (currentSlide < slides.Length - 1)
        {
            currentSlide++;
            UpdateSlide();
        }
        else
        {
            CompleteTutorial();
        }
    }
    
    private void CloseTutorial()
    {
        CompleteTutorial();
    }
    
    private void CompleteTutorial()
    {
        SetTutorialCompleted();
        tutorialPanel.SetActive(false);
    }
    
    public void ResetTutorial()
    {
        PlayerPrefs.DeleteKey(TUTORIAL_COMPLETED_KEY);
        PlayerPrefs.Save();
        ShowTutorial();
    }
}