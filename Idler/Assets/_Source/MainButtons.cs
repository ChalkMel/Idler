using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Serialization;

public class MainButtons : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button shopButton;
    [SerializeField] private Button frogButton;
    [SerializeField] private Button deerButton;
    [SerializeField] private float duration;
    [SerializeField] private Button shopCloseButton;
    [SerializeField] private List<Button> closeButton;
    [Header("Frog")]
    [SerializeField] private CanvasGroup frogUI;
    [SerializeField] private List<string> frogTalks;
    [SerializeField] private TextMeshProUGUI frogUIText;
    [Header("Deer")]
    [SerializeField] private CanvasGroup TeaUI;
    [SerializeField] private List<string> deerTalks;
    [SerializeField] private CanvasGroup DeerUI;
    [SerializeField] private TextMeshProUGUI deerUIText;
    [SerializeField] private TeaMaker teaMaker;
    [Header("Shop")]
    [SerializeField] private CanvasGroup shopUI;
    
    private Tween _fadeTween;
    private System.Random _random;
    private void Start()
    {
        _random = new System.Random();
        shopButton.onClick.AddListener(OpenShopUI);
        frogButton.onClick.AddListener(OpenFrogUI);
        deerButton.onClick.AddListener(OpenDeerUI);
        shopCloseButton.onClick.AddListener(CloseShopUI);
        foreach (Button t in closeButton)
        {
            t.onClick.AddListener(CloseUI);
        }
        
    }
    
    private void OpenFrogUI()
    {
        Open(frogUI);
        int random = _random.Next(0, frogTalks.Count);
        frogUIText.text = frogTalks[random];
    }

    private void Open(CanvasGroup canvasGroup)
    {
        CloseUI();
        WindowHelper.Show(canvasGroup,0);
    }
    private void OpenDeerUI()
    {
        Open(TeaUI);
        WindowHelper.Show(DeerUI, 0.5f);
        deerUIText.text = deerTalks[_random.Next(0, deerTalks.Count)];
        teaMaker.UpdateCounters();
    }
    
    private void OpenShopUI()
    {
        WindowHelper.ShowWithSlide(shopUI, 1f, new Vector2(1000f, 0f));
    }

    private void CloseShopUI()
    {
        WindowHelper.HideWithSlide(shopUI, 1f, new Vector2(1000f, 0f));
    }
    private void CloseUI()
    {
        _fadeTween?.Kill();
        TeaUI.gameObject.SetActive(false);
        frogUI.gameObject.SetActive(false);
        shopUI.gameObject.SetActive(false);
    }
}
