using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public bool triggered;
    public float timerReduction;
    public List<GameObject> bystanders;
    
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
            GameObject possessed = bystanders[Random.Range(0, bystanders.Count)];
            //Ajouter le code pour le possédé
        }
    }

    private IEnumerator TimerHandler()
    {
        yield return new WaitForSeconds(10f);
        timerReduction += 0.5f;
    }

}
