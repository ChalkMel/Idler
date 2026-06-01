using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class PlayOnClick : MonoBehaviour
{
    private Button  _button;
    private AudioSource _audioSource;

    private void Start()
    {
        _button = GetComponent<Button>();
        _audioSource = GetComponent<AudioSource>();
        _button.onClick.AddListener(Play);
    }

    private void Play()
    {
        _audioSource.Play();
    }
}
