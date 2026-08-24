using UnityEngine;

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
        PlaySFX(clics[Random.Range(0, clics.Length)]);
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
