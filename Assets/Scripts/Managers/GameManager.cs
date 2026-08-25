using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>
{
    public bool triggeredFrenzy;
    public float timerReduction;
    public List<BystanderScript> bystanders;
    public int dayCount = 2;
    public float timeLeft;
    private IEnumerator runningTimer;
    private bool dayEnded;

    
    void Start()
    {
        NextDay();
    }


    private IEnumerator Possession()
    {
        while (triggeredFrenzy == false && bystanders.Count != 0)
        {
            yield return new WaitForSeconds(Random.Range(7f-timerReduction, 10f-timerReduction));
            BystanderScript possessed = bystanders[Random.Range(0, bystanders.Count)];
            IEnumerator chosenCoroutine;
            chosenCoroutine = possessed.Possession();
            if (dayCount >= 2)
            {
                if (Random.Range(0f, 3f) <1)
                {
                    chosenCoroutine = possessed.Fakeout();
                }else if (dayCount >= 3 && Random.Range(0f, 4f) <1)
                {
                    chosenCoroutine = possessed.Feral();
                }
            }
            if (dayCount == 4)
            {
                chosenCoroutine = possessed.Smart();
            }
            StartCoroutine(chosenCoroutine);
        }

        if (bystanders.Count == 0)
        {
            StopCoroutine(runningTimer);
            ChooseDay();
        }
    }

    private IEnumerator TimerHandler()
    {
        while (!dayEnded)
        {
            yield return new WaitForSeconds(10f);
            timerReduction += 0.5f;
        }
    }

    public void RemoveBystander(BystanderScript bystander)
    {
        bystanders.Remove(bystander);
    }

    public void Frenzy()
    {
        foreach (BystanderScript bystander in bystanders)
        {
            bystander.Frenzy();
        }
    }

    public void FindBystanders()
    {
        bystanders.Clear();
        foreach (BystanderScript bystander in FindObjectsOfType<BystanderScript>())
        {
            bystanders.Add(bystander);
        }
    }
    
    public void NextDay()
    {
        FindBystanders();
        triggeredFrenzy = false;
        timerReduction = 0;
        StartCoroutine(Possession());
        StartCoroutine(TimerHandler());
        dayCount++;
        timeLeft = 120f;
        runningTimer = GameDuration(timeLeft);
        StartCoroutine(runningTimer);
        dayEnded = false;
    }

    public IEnumerator GameDuration(float time)
    {
        yield return new WaitForSeconds(time);
        ChooseDay();
    }


    public void ChooseDay()
    {
        if(dayEnded)
            return;
        dayEnded = true;
        switch (dayCount)
        {
            case 1:
                //Go to day 2
                NextDay();
                break;
            case 2:
                //Go to day 3
                NextDay();
                break;
            case 3:
                //Go to day 4
                NextDay();
                break;
            case 4:
                //Go to day 5
                NextDay();
                break;
            case 5:
                //End game
                break;
        }
        
    }
}
