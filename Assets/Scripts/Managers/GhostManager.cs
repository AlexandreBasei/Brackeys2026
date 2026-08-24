using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public bool triggered;
    public float timerReduction;
    public List<BystanderScript> bystanders;
    
    void Start()
    {
        triggered = false;
        timerReduction = 0;
        StartCoroutine(Possession());
        StartCoroutine(TimerHandler());
    }


    private IEnumerator Possession()
    {
        while (triggered == false & bystanders.Count == 0)
        {
            yield return new WaitForSeconds(Random.Range(7f-timerReduction, 10f-timerReduction));
            BystanderScript possessed = bystanders[Random.Range(0, bystanders.Count)];
            StartCoroutine(Possession());
        }
    }

    private IEnumerator TimerHandler()
    {
        yield return new WaitForSeconds(10f);
        timerReduction += 0.5f;
    }

    public void RemoveBystander(BystanderScript bystander)
    {
        bystanders.Remove(bystander);
    }

}
