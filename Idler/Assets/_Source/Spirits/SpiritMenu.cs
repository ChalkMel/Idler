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

        if (spiritCollection.unlockedSpirits.Count > 0)
        {
            ShowSpiritDetails(spiritCollection.unlockedSpirits[0]);
        }
        else if (spiritCollection.allSpirits.Count > 0)
        {
            ShowSpiritDetails(spiritCollection.allSpirits[0]);
        }
    }
    
    private void ShowErrorMessage(string message)
    {
        if (selectedSpiritName != null)
            selectedSpiritName.text = message;
        
        if (selectedSpiritDescription != null)
            selectedSpiritDescription.text = "";
        
        if (selectedSpiritBuff != null)
            selectedSpiritBuff.text = "";
        
        ClearPanel(likedTeasPanel);
    }
    
    private void CreateSpiritButtons()
    {
        
        foreach (var spirit in spiritCollection.allSpirits)
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
                        buttonImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
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
                if (spirit.icon != null)
                {
                    image.sprite = spirit.icon;
                    image.preserveAspect = true;
                    
                    if (!spirit.isUnlocked)
                    {
                        image.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                    }
                }
                break;
            }
        }
        
        
        if (buttonObj.GetComponent<Image>() != null && spirit.icon != null)
        {
            buttonObj.GetComponent<Image>().sprite = spirit.icon;
            
            if (!spirit.isUnlocked)
            {
                buttonObj.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
        }
        
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = spirit.spiritName;
        }
    }
    
    private void ShowSpiritDetails(SpiritData spirit)
    {
        if (spirit == null) return;
        
        if (selectedSpiritIcon != null)
        {
            if (spirit.icon != null)
            {
                selectedSpiritIcon.sprite = spirit.icon;
                selectedSpiritIcon.preserveAspect = true;

                selectedSpiritIcon.color = spirit.isUnlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
        }
        
        if (selectedSpiritName != null)
        {
            selectedSpiritName.text = spirit.spiritName;
            if (!spirit.isUnlocked)
            {
                selectedSpiritName.text += " (Заблокирован)";
            }
        }
        
        if (selectedSpiritDescription != null)
        {
            selectedSpiritDescription.text = spirit.description;
        }
        
        if (selectedSpiritBuff != null)
        {
            if (spirit.isUnlocked)
            {
                selectedSpiritBuff.text = $"Буст: {spirit.buffName}\n{spirit.buffDescription}\nМножитель: {spirit.buffMultiplier}x";
            }
            else
            {
                selectedSpiritBuff.text = "Этот дух еще не найден";
            }
        }

        ClearPanel(likedTeasPanel);

        if (spirit.likedTeas is {Length: > 0})
        {
            foreach (var tea in spirit.likedTeas)
            {
                if (tea == null) continue;
                
                CreateTeaIconInPanel(likedTeasPanel, tea);
            }
        }
        else
        {
            CreateTextInPanel(likedTeasPanel, "Не любит никакие чаи");
        }
    }
    
    private void CreateTeaIconInPanel(Transform panel, TeaData tea)
    {
        if (panel == null || teaIconPrefab == null) return;
        
        GameObject iconObj = Instantiate(teaIconPrefab, panel);

        Image iconImage = iconObj.GetComponent<Image>();
        if (iconImage != null && tea.icon != null)
        {
            iconImage.sprite = tea.icon;
            iconImage.preserveAspect = true;
        }

        TextMeshProUGUI text = iconObj.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = tea.teaName;
            text.fontSize = 10;
        }
    }
    
    private void CreateTextInPanel(Transform panel, string text)
    {
        if (panel == null) return;
        
        GameObject textObj = new GameObject("InfoText");
        textObj.transform.SetParent(panel);
        
        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = 14;
        textComponent.color = Color.gray;
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