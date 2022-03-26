using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic Instance;
    [SerializeField] private AudioClip MainMenuMusic;
    [SerializeField] private AudioClip GameplayMusic;
    private AudioSource audioSource;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    public void PlayGamePlay()
    {
        audioSource.clip = GameplayMusic;
        audioSource.Play();
    }

    public void PlayMainMenu()
    {
        audioSource.clip = MainMenuMusic;
        audioSource.Play();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

}
