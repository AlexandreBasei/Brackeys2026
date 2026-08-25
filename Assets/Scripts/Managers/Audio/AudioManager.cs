using UnityEngine;
using NaughtyAttributes;

public class AudioManager : PersistentSingleton<AudioManager>
{
    [Header("-------AudioSource-------")]
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource SFX;

    [Header("-------AudioClipSFX-------")]
    public AudioClip[] clics;
    [SerializeField] private AudioClip[] GunShots;
    [SerializeField] private AudioClip GunReload;

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
        // PlayMusic(MusicMainMenu);
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
        PlaySFX(clics[Random.Range(0, clics.Length)]);
    }

    public void PlayStep()
    {
        int randomIndex = Random.Range(0, 3);
        AudioClip stepClip = null;

        switch (randomIndex)
        {
            case 0:
                stepClip = step1;
                break;
            case 1:
                stepClip = step2;
                break;
            case 2:
                stepClip = step3;
                break;
        }

        if (stepClip != null)
        {
            PlaySFX(stepClip);
        }
    }

    public void PlayShoot()
    {
        PlaySFX(GunShots[Random.Range(0, GunShots.Length)]);
    }

    public void PlayReload()
    {
        PlaySFX(GunReload);
    }
}
