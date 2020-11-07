using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : MonoBehaviour
{
    Random rnd = new Random();
    public AudioSource audioSource;
    public AudioClip btnClickClip;
    public AudioClip gameTypeClip;
    public AudioClip gameLoseClip;
    public AudioClip gameQuitClip;
    public AudioClip gameWinClip;
    public List<AudioClip> gameClips;
    private int currentClipIndex=0;
    void Start()
    {
        if(GameMain.optionData != null)
        {
            SetAudioClipVolume();
            if (gameClips != null)
            {
                currentClipIndex = Random.Range(0, gameClips.Count);
                audioSource.PlayOneShot(gameClips[currentClipIndex]);
            }
        }
        
    }
    private void Update()
    {
        if (gameClips != null && !audioSource.isPlaying)
        {
            currentClipIndex = Random.Range(0, gameClips.Count);
            audioSource.PlayOneShot(gameClips[currentClipIndex]);
        }
    }
    public void SetAudioClipVolume()
    {
        int value = GameMain.optionData.volumeLevel;
        float volume = ((float)value) / 100;
        audioSource.volume = volume;
    }
    public void ButtonClickClipPlay()
    {
        audioSource.PlayOneShot(btnClickClip);
    }
    public void GameTypeClipPlay()
    {
        audioSource.PlayOneShot(gameTypeClip);
    }
    public void GameLoseClipPlay()
    {
        audioSource.PlayOneShot(gameLoseClip);
    }
    public void GameQuitClipPlay()
    {
        audioSource.PlayOneShot(gameQuitClip);
    }
    public void GameWinClipPlay()
    {
        audioSource.PlayOneShot(gameWinClip);
    }
}
