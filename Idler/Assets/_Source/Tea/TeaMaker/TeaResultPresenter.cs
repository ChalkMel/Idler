using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TeaResultPresenter : MonoBehaviour
{
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private Image _resultTeaIcon;
    [SerializeField] private TextMeshProUGUI _resultTeaName;
    [SerializeField] private TextMeshProUGUI _resultTeaDescription;
    [SerializeField] private Image _resultSpiritIcon;
    [SerializeField] private TextMeshProUGUI _resultSpiritName;
    [SerializeField] private TextMeshProUGUI _resultBuffInfo;
    [SerializeField] private float _displayDuration = 5f;

    public void ShowResult(TeaData tea, SpiritData spirit)
    {
        if (_resultPanel == null) return;

        if (_resultTeaIcon != null && tea.Icon != null)
        {
            _resultTeaIcon.sprite = tea.Icon;
            _resultTeaIcon.gameObject.SetActive(true);
        }
        if (_resultTeaName != null)
        {
            _resultTeaName.text = $"Вы сварили:\n{tea.TeaName}";
            _resultTeaName.gameObject.SetActive(true);
        }
        if (_resultTeaDescription != null)
        {
            _resultTeaDescription.text = tea.Description;
            _resultTeaDescription.gameObject.SetActive(true);
        }
        if (_resultSpiritIcon != null && spirit.Icon != null)
        {
            _resultSpiritIcon.sprite = spirit.Icon;
            _resultSpiritIcon.gameObject.SetActive(true);
        }
        if (_resultSpiritName != null)
        {
            _resultSpiritName.text = $"Пришел:\n{spirit.SpiritName}";
            _resultSpiritName.gameObject.SetActive(true);
        }
        if (_resultBuffInfo != null)
        {
            _resultBuffInfo.text = $"Буст: {spirit.BuffName}\nСила: x{spirit.BuffMultiplier:F1}\nВремя: {spirit.BuffDuration}с";
            _resultBuffInfo.gameObject.SetActive(true);
        }

        _resultPanel.SetActive(true);
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(_displayDuration);
        _resultPanel.SetActive(false);

        if (_resultTeaIcon != null) _resultTeaIcon.gameObject.SetActive(false);
        if (_resultTeaName != null) _resultTeaName.gameObject.SetActive(false);
        if (_resultTeaDescription != null) _resultTeaDescription.gameObject.SetActive(false);
        if (_resultSpiritIcon != null) _resultSpiritIcon.gameObject.SetActive(false);
        if (_resultSpiritName != null) _resultSpiritName.gameObject.SetActive(false);
        if (_resultBuffInfo != null) _resultBuffInfo.gameObject.SetActive(false);
    }
}