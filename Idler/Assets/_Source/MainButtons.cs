using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MainButtons : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button shopButton;
    [SerializeField] private Button frogButton;
    [SerializeField] private Button deerButton;
    [SerializeField] private float duration;
    [SerializeField] private List<Button> closeButton;
    [Header("Frog")]
    [SerializeField] private CanvasGroup frogUI;
    [SerializeField] private List<string> frogTalks;
    [SerializeField] private TextMeshProUGUI frogUIText;
    [Header("Deer")]
    [SerializeField] private CanvasGroup deerUI;
    [SerializeField] private List<string> deerTalks;
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
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        _fadeTween = canvasGroup.DOFade(1f, duration);
    }
    private void OpenDeerUI()
    {
        Open(deerUI);
        teaMaker.UpdateCounters();
    }
    
    private void OpenShopUI()
    {
        Open(shopUI);
    }

    private void CloseUI()
    {
        _fadeTween?.Kill();
        deerUI.gameObject.SetActive(false);
        frogUI.gameObject.SetActive(false);
        shopUI.gameObject.SetActive(false);
    }
}
