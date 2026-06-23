using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpiritMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpiritCollection spiritCollection;
    
    [Header("UI Elements")]
    [SerializeField] private Transform spiritListParent; 
    [SerializeField] private GameObject spiritButtonPrefab; 
    [SerializeField] private Image selectedSpiritIcon; 
    [SerializeField] private TextMeshProUGUI selectedSpiritName; 
    [SerializeField] private TextMeshProUGUI selectedSpiritDescription;
    [SerializeField] private TextMeshProUGUI selectedSpiritBuff; 
    [SerializeField] private Transform likedTeasPanel; 
    [SerializeField] private GameObject teaIconPrefab;
    
    [Header("Settings")]
    [SerializeField] private bool startHidden = true;

    [SerializeField] private Color butColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    
    private List<GameObject> _spiritButtons = new List<GameObject>();
    
    private void Start()
    {
        if (startHidden)
        {
            gameObject.SetActive(false);
        }
    }
    
    public void OpenMenu()
    {
        gameObject.SetActive(true);
        RefreshMenu();
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    private void RefreshMenu()
    {
        ClearAllButtons();
        
        CreateSpiritButtons();

        if (spiritCollection.UnlockedSpirits.Count > 0)
        {
            ShowSpiritDetails(spiritCollection.UnlockedSpirits[0]);
        }
        else if (spiritCollection.AllSpirits.Count > 0)
        {
            ShowSpiritDetails(spiritCollection.AllSpirits[0]);
        }
    }

    private void CreateSpiritButtons()
    {
        
        foreach (var spirit in spiritCollection.AllSpirits)
        {
            if (spirit == null) continue;

            GameObject buttonObj = Instantiate(spiritButtonPrefab, spiritListParent);
            _spiritButtons.Add(buttonObj);

            SetupSpiritButton(buttonObj, spirit);

            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                SpiritData currentSpirit = spirit;
                button.onClick.AddListener(() => ShowSpiritDetails(currentSpirit));

                if (!spirit.isUnlocked)
                {
                    button.interactable = false;

                    Image buttonImage = buttonObj.GetComponent<Image>();
                    if (buttonImage != null)
                    {
                        buttonImage.color = butColor;
                    }
                }
            }
        }
    }
    
    private void SetupSpiritButton(GameObject buttonObj, SpiritData spirit)
    {
        Image[] images = buttonObj.GetComponentsInChildren<Image>();
        
        foreach (var image in images)
        {
            if (image.transform.parent == buttonObj.transform)
            {
                if (spirit.Icon != null)
                {
                    image.sprite = spirit.Icon;
                    image.preserveAspect = true;
                    
                    if (!spirit.isUnlocked)
                    {
                        image.color = butColor;
                    }
                }
                break;
            }
        }
        
        
        if (buttonObj.GetComponent<Image>() != null && spirit.Icon != null)
        {
            
            if (!spirit.isUnlocked)
            {
                buttonObj.GetComponent<Image>().color = butColor;
            }
        }
        
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = spirit.SpiritName;
        }
    }
    
    private void ShowSpiritDetails(SpiritData spirit)
    {
        if (spirit == null) return;
        
        if (selectedSpiritIcon != null)
        {
            if (spirit.Icon != null)
            {
                selectedSpiritIcon.sprite = spirit.Icon;
                selectedSpiritIcon.preserveAspect = true;

                selectedSpiritIcon.color = spirit.isUnlocked ? Color.white : butColor;
            }
        }
        
        if (selectedSpiritName != null)
        {
            selectedSpiritName.text = spirit.SpiritName;
            if (!spirit.isUnlocked)
            {
                selectedSpiritName.text += " закрыт";
            }
        }
        
        if (selectedSpiritDescription != null)
        {
            selectedSpiritDescription.text = spirit.Description;
        }
        
        if (selectedSpiritBuff != null)
        {
            selectedSpiritBuff.text = spirit.isUnlocked ? $"{spirit.BuffName}\n{spirit.BuffDescription}" : "Ещё не нашли";
        }

        ClearPanel(likedTeasPanel);

        if (spirit.LikedTeas is {Length: > 0})
        {
            foreach (var tea in spirit.LikedTeas)
            {
                if (tea == null) continue;
                
                CreateTeaIconInPanel(likedTeasPanel, tea);
            }
        }
    }
    
    private void CreateTeaIconInPanel(Transform panel, TeaData tea)
    {
        if (panel == null || teaIconPrefab == null) return;
        
        GameObject iconObj = Instantiate(teaIconPrefab, panel);

        Image iconImage = iconObj.GetComponent<Image>();
        if (iconImage != null && tea.Icon != null)
        {
            iconImage.sprite = tea.Icon;
            iconImage.preserveAspect = true;
        }
    }
    
    private void ClearPanel(Transform panel)
    {
        if (panel == null) return;
        
        foreach (Transform child in panel)
        {
            Destroy(child.gameObject);
        }
    }
    
    private void ClearAllButtons()
    {
        foreach (var button in _spiritButtons)
        {
            if (button != null)
                Destroy(button);
        }
        _spiritButtons.Clear();
    }
    
    private void OnDestroy()
    {
        ClearAllButtons();
    }
}