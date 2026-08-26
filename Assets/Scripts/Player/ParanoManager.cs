using UnityEngine;
using System.Collections;

public class ParanoManager : MonoBehaviour
{
    [Header("-------AudioSource-------")]
    [SerializeField] private AudioSource SourceLeft;
    [SerializeField] private AudioSource SourceRight;
    [SerializeField] private AudioSource SourceBack;

    [Header("-------AudioClipSFX-------")]

    [SerializeField] private AudioClip[] paranoSounds;



    [Header("-------Random Delay-------")]
    [SerializeField] private float minDelay = 45f;
    [SerializeField] private float maxDelay = 120f;

    void Start()
    {
        StartCoroutine(PlayRandomSound());
    }

    public void PlaySFX(AudioClip clip)
    {
        int randomIndex = Random.Range(0, 3);
        switch (randomIndex)
        {
            case 0:
                SourceLeft.PlayOneShot(clip);
                break;
            case 1:
                SourceRight.PlayOneShot(clip);
                break;
            case 2:
                SourceBack.PlayOneShot(clip);
                break;
        }
    }

    

    private IEnumerator PlayRandomSound()
    {
       while(true)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            PlaySFX(paranoSounds[Random.Range(0, paranoSounds.Length)]);
        } 
    }
}
