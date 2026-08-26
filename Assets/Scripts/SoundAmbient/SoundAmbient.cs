using UnityEngine;
using System.Collections;

public class SoundAmbient : MonoBehaviour
{
    [Header("-------Audio Source-------")]
    [SerializeField] private AudioSource[] audioSource;

    [Header("-------Audio Clips-------")]
    [SerializeField] private AudioClip TVSound;
    [SerializeField] private AudioClip PianoSound;
    [SerializeField] private AudioClip GramophoneSound;

    [Header("-------Random Delay-------")]
    [SerializeField] private float minDelay = 45f;
    [SerializeField] private float maxDelay = 120f;
    
    void Start()
    {
        StartCoroutine(PlayRandomSound());
    }

    public void PlayAmbiant()
    {
        int randomIndex = Random.Range(0, (audioSource.Length)+1);
        switch (randomIndex)
        {
            case 0:
                audioSource[0].PlayOneShot(TVSound);
                break;
            case 1:
                audioSource[1].PlayOneShot(TVSound);
                break;
            case 2:
                audioSource[2].PlayOneShot(PianoSound);
                break;
            case 3:
                audioSource[3].PlayOneShot(GramophoneSound);
                break;
            case 4:
                audioSource[4].PlayOneShot(GramophoneSound);
                break;
        }
    }


    private IEnumerator PlayRandomSound()
    {
       while(true)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            PlayAmbiant();
        } 
    }
}
