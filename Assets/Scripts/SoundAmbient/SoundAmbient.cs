using UnityEngine;
using System.Collections;

public class SoundAmbient : MonoBehaviour
{
    [Header("-------Audio Source-------")]
    [SerializeField] private AudioSource audioSource;

    [Header("-------Audio Clips-------")]
    [SerializeField] private AudioClip audioClips;

    [Header("-------Random Delay-------")]
    [SerializeField] private float minDelay = 45f;
    [SerializeField] private float maxDelay = 120f;
    
    void Start()
    {
        StartCoroutine(PlayRandomSound());
    }

    private IEnumerator PlayRandomSound()
    {
       while(true)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            audioSource.PlayOneShot(audioClips);
        } 
    }
}
