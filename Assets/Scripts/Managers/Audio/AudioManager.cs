using UnityEngine;
using NaughtyAttributes;

public class AudioManager : PersistentSingleton<AudioManager>
{
    [Header("-------AudioSource-------")]
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource SFX;


    [Header("-------AudioClipMusic-------")]
    public AudioClip MusicMainMenu;
    public AudioClip MusicInGame;
    public AudioClip MusicEndGame;



    [Foldout("AudioClipSFX/Step")]
    public AudioClip step1;
    [Foldout("AudioClipSFX/Step")]
    public AudioClip step2;
    [Foldout("AudioClipSFX/Step")]
    public AudioClip step3;
    [Foldout("AudioClipSFX/Gun sound")]
    public AudioClip shoot1;

    [Foldout("AudioClipSFX/Gun sound")]
    public AudioClip shoot2;

    [Foldout("AudioClipSFX/Gun sound")]
    public AudioClip reload;

    [Foldout("AudioClipSFX/Amiance Sound and Event")]
    public AudioClip whisper1;

    [Foldout("AudioClipSFX/Amiance Sound and Event")]
    public AudioClip whisper2;

    [Foldout("AudioClipSFX/Amiance Sound and Event")]
    public AudioClip laught;

    [Foldout("AudioClipSFX/Amiance Sound and Event")]
    public AudioClip clock;

    [Foldout("AudioClipSFX/Amiance Sound and Event")]
    public AudioClip cough;

    [Foldout("AudioClipSFX/Amiance Sound and Event")]
    public AudioClip voice1;

    [Foldout("AudioClipSFX/Amiance Sound and Event")]
    public AudioClip voice2;

    [Foldout("AudioClipSFX/Amiance Sound and Event")]
    public AudioClip piano;

    [Foldout("AudioClipSFX/Amiance Sound and Event")]
    public AudioClip choir;




    void Start()
    {
        PlayMusic(MusicMainMenu);
    }

    public void PlaySFX(AudioClip clip)
    {
        SFX.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip musicClip)
    {
        music.clip = musicClip;
        music.loop = true;
        music.mute = false;
        music.Play();
    }

    public void StopMusic()
    {
        music.mute = true;
    }

    public void PlayClic()
    {
        var id = Random.Range(1, 4);
        switch (id)
        {
            case 1:
                PlaySFX(step1);
                break;
            case 2:
                PlaySFX(step2);
                break;
            case 3:
                PlaySFX(step3);
                break;
        }

    }
}
