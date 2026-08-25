using UnityEngine;
using System.Collections;

public class ClockSound : MonoBehaviour
{
    [Header("-------Audio Source-------")]
    [SerializeField] private AudioSource audioSource;

    [Header("-------Audio Clips-------")]
    [SerializeField] private AudioClip audioClips;

    [Header("-------Random Delay-------")]
    [SerializeField] private float delay = 60f;

    
    void Start()
    {
        StartCoroutine(PlayRandomSound());
    }

    private IEnumerator PlayRandomSound()
    {
       while(true)
        {
            yield return new WaitForSeconds(delay);

            audioSource.PlayOneShot(audioClips);
        } 
    }
}
