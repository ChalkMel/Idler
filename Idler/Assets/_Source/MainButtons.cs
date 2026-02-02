using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
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
    [Header("Deer")]
    [SerializeField] private CanvasGroup deerUI;
    [Header("Shop")]
    [SerializeField] private CanvasGroup shopUI;
    
    private Tween _fadeTween;
    private void Start()
    {
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
